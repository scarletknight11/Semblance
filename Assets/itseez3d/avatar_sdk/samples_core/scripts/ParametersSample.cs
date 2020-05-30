/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, April 2017
*/

using System.Collections;
using System.Collections.Generic;
using ItSeez3D.AvatarSdk.Core;
using ItSeez3D.AvatarSdkSamples.SamplePipelineTraits;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ItSeez3D.AvatarSdkSamples.Core
{
	public class ParametersSample : GettingStartedSample
	{
		#region UI elements
		public GameObject haircutsPanel;
		public GameObject blendshapesPanel;
		public GameObject modelInfoPanel;
		public GameObject avatarModificationsPanel;
		public GameObject shapeModificationsPanel;
		public GameObject additionalTexturesPanel;

		public Button haircutsButton;
		public Button blendshapesButton;
		public Button modelInfoButton;
		public Button avatarModificationsButton;
		public Button shapeModificationsButton;
		public Button additionalTexturesButton;		

		public ItemsSelectingView haircutsSelectingView;
		public ItemsSelectingView blendshapesSelectingView;
		public ModelInfoParametersPanel modelInfoParametersPanel;
		public AvatarModificationsParametersPanel avatarModificationsParametersPanel;
		public ShapeModificationsParametersPanel shapeModificationsParametersPanel;
		public ItemsSelectingView additionalTexturesSelectingView;
		#endregion

		#region private members
		private List<GameObject> panels = null;
		#endregion


		#region public methods
		public override void OnPipelineTypeToggleChanged(PipelineType pipelineType)
		{
			base.OnPipelineTypeToggleChanged(pipelineType);

			if (avatarProvider != null && avatarProvider.IsInitialized)
				StartCoroutine(UpdateAvatarParameters());
		}
		#endregion

		#region protected methods
		protected override void Start()
		{
			panels = new List<GameObject>()
			{
				haircutsPanel,
				blendshapesPanel,
				modelInfoPanel,
				avatarModificationsPanel,
				additionalTexturesPanel
			};
			if (shapeModificationsPanel != null)
				panels.Add(shapeModificationsPanel);

			base.Start();

			haircutsButton.onClick.AddListener(() => OnShowPanelButtonClick(haircutsPanel));
			blendshapesButton.onClick.AddListener(() => OnShowPanelButtonClick(blendshapesPanel));
			modelInfoButton.onClick.AddListener(() => OnShowPanelButtonClick(modelInfoPanel));
			avatarModificationsButton.onClick.AddListener(() => OnShowPanelButtonClick(avatarModificationsPanel));
			if (shapeModificationsButton != null)
				shapeModificationsButton.onClick.AddListener(() => OnShowPanelButtonClick(shapeModificationsPanel));
			additionalTexturesButton.onClick.AddListener(() => OnShowPanelButtonClick(additionalTexturesPanel));
		}

		/// <summary>
		/// Initializes avatar provider and requests available parameters
		/// </summary>
		protected override IEnumerator Initialize()
		{
			avatarProvider = AvatarSdkMgr.GetAvatarProvider();
			if (!avatarProvider.IsInitialized)
			{
				var initializeRequest = avatarProvider.InitializeAsync();
				yield return Await(initializeRequest);
				if (initializeRequest.IsError)
				{
					Debug.LogError("Avatar provider was not initialized!");
					yield break;
				}
			}

			yield return CheckAvailablePipelines();

			yield return UpdateAvatarParameters();
		}

		protected override IEnumerator ConfigureComputationParameters(PipelineType pipelineType, ComputationParameters computationParameters)
		{
			computationParameters.haircuts = new ComputationList(haircutsSelectingView.CurrentSelection);
			computationParameters.blendshapes = new ComputationList(blendshapesSelectingView.CurrentSelection);
			computationParameters.modelInfo = modelInfoParametersPanel.GetParameters();
			computationParameters.avatarModifications = avatarModificationsParametersPanel.GetParameters();
			if (shapeModificationsPanel != null)
				computationParameters.shapeModifications = shapeModificationsParametersPanel.GetParameters();
			computationParameters.additionalTextures = new ComputationList(additionalTexturesSelectingView.CurrentSelection);
			yield break;
		}

		/// <summary>
		/// Generates avatar with the selected set of parameters and displayed it in the AvatarViewer scene
		/// </summary>
		protected override IEnumerator GenerateAndDisplayHead(byte[] photoBytes, PipelineType pipeline)
		{
			ComputationParameters computationParameters = ComputationParameters.Empty;
			yield return ConfigureComputationParameters(pipeline, computationParameters);

			var initializeRequest = avatarProvider.InitializeAvatarAsync(photoBytes, "name", null, pipeline, computationParameters);
			yield return Await(initializeRequest);
			string avatarCode = initializeRequest.Result;

			StartCoroutine(SampleUtils.DisplayPhotoPreview(avatarCode, photoPreview));

			var calculateRequest = avatarProvider.StartAndAwaitAvatarCalculationAsync(avatarCode);
			yield return Await(calculateRequest);

			//Download avatar mesh, blendshapes and additional textures if it is Cloud version
			if (sdkType == SdkType.Cloud)
			{
				var downloadDataRequest = avatarProvider.MoveAvatarModelToLocalStorageAsync(avatarCode, false, computationParameters.blendshapes.Values.Count != 0);
				yield return Await(downloadDataRequest);

				if (computationParameters.additionalTextures.Values.Count > 0)
				{
					List<AsyncRequest> downloadTexturesRequests = new List<AsyncRequest>();
					foreach(var texture in computationParameters.additionalTextures.Values)
					{
						var request = avatarProvider.GetTextureAsync(avatarCode, texture.Name);
						downloadTexturesRequests.Add(request);
					}
					yield return Await(downloadTexturesRequests.ToArray());
				}
			}

			AvatarViewer.SetSceneParams(new AvatarViewer.SceneParams()
			{
				avatarCode = avatarCode,
				showSettings = false,
				sceneToReturn = SceneManager.GetActiveScene().name,
				avatarProvider = avatarProvider,
				useAnimations = false
			});
			SceneManager.LoadScene(PluginStructure.GetScenePath(SampleScene.AVATAR_VIEWER));
		}

		protected override void SetControlsInteractable(bool interactable)
		{
			base.SetControlsInteractable(interactable);

			foreach (GameObject obj in panels)
			{
				foreach (Selectable c in obj.GetComponentsInChildren<Selectable>())
				{
					ControlEnabling controlEnabling = c.GetComponent<ControlEnabling>();
					if (controlEnabling == null)
						controlEnabling = c.gameObject.transform.parent.GetComponent<ControlEnabling>();
					if (controlEnabling == null || controlEnabling.isEnabled)
						c.interactable = interactable;
				}
			}
		}

		protected IEnumerator UpdateAvatarParameters()
		{
			if (avatarProvider == null)
				yield break;

			SetControlsInteractable(false);

			// Get all available parameters
			var allParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.ALL, selectedPipelineType);
			// Get default parameters
			var defaultParametersRequest = avatarProvider.GetParametersAsync(ComputationParametersSubset.DEFAULT, selectedPipelineType);
			yield return Await(allParametersRequest, defaultParametersRequest);

			if (allParametersRequest.IsError || defaultParametersRequest.IsError)
			{
				Debug.LogError("Unable to get parameters list");
				haircutsSelectingView.InitItems(new List<string>());
				blendshapesSelectingView.InitItems(new List<string>());
				modelInfoParametersPanel.UpdateParameters(new ModelInfoGroup(), new ModelInfoGroup());
				avatarModificationsParametersPanel.UpdateParameters(new AvatarModificationsGroup(), new AvatarModificationsGroup());
				additionalTexturesSelectingView.InitItems(new List<string>());
				if (shapeModificationsParametersPanel != null)
					shapeModificationsParametersPanel.UpdateParameters(new ShapeModificationsGroup(), new ShapeModificationsGroup());
			}
			else
			{
				ComputationParameters allParameters = allParametersRequest.Result;
				ComputationParameters defaultParameters = defaultParametersRequest.Result;

				haircutsSelectingView.InitItems(allParameters.haircuts.FullNames, defaultParameters.haircuts.FullNames);
				blendshapesSelectingView.InitItems(allParameters.blendshapes.FullNames, defaultParameters.blendshapes.FullNames);
				modelInfoParametersPanel.UpdateParameters(allParameters.modelInfo, defaultParameters.modelInfo);
				avatarModificationsParametersPanel.UpdateParameters(allParameters.avatarModifications, defaultParameters.avatarModifications);
				additionalTexturesSelectingView.InitItems(allParameters.additionalTextures.FullNames, defaultParameters.additionalTextures.FullNames);
				if (shapeModificationsParametersPanel != null)
					shapeModificationsParametersPanel.UpdateParameters(allParameters.shapeModifications, defaultParameters.shapeModifications);
			}

			SetControlsInteractable(true);
		}

		protected void OnShowPanelButtonClick(GameObject activePanel)
		{
			foreach(GameObject panel in panels)
			{
				if (panel != activePanel)
					panel.SetActive(false);
			}
			activePanel.SetActive(true);
		}

		protected override IEnumerator CheckAvailablePipelines()
		{
			yield return base.CheckAvailablePipelines();

			var cartoonishPipelineAvailabilityRequest = avatarProvider.IsPipelineSupportedAsync(PipelineType.STYLED_FACE);
			yield return Await(cartoonishPipelineAvailabilityRequest);
			if (cartoonishPipelineAvailabilityRequest.IsError)
				yield break;
		}

		protected override void OnDestroy()
		{
			
		}
		#endregion
	}
}
