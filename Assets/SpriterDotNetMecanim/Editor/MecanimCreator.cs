using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using SpriterDotNet;
using SpriterDotNetUnity;
using System;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace SpriterDotNetMecanim.Editor {
	/// <summary>
	/// Responsible for the postprocessing of the .scml import. Creates the AnimatorController.
	/// </summary>
	[InitializeOnLoad] public static class MecanimCreator {
		static MecanimCreator () {
			SpriterImporter.EntityImported += Process;
		}

		/// <summary>
		/// Subscribe to this event for further postprocessing
		/// </summary>
		public static event Action<SpriterEntity, GameObject, AnimatorController> MecanimDone;

		private static void Process (SpriterEntity entity, GameObject prefab) {
			GetAssistant (prefab);
			var controller = GetController (prefab);
			ProcessAnimations (entity.Animations, controller);
			if (MecanimDone != null) MecanimDone (entity, prefab, controller);

		}

		private static AnimatorAssist GetAssistant (GameObject prefab) {
			var assistant = prefab.GetComponent<AnimatorAssist> ();
			if (!assistant) assistant = prefab.AddComponent<AnimatorAssist> ();
			return assistant;
		}

		private static AnimatorController GetController (GameObject prefab) {
			var animator = prefab.GetComponent<Animator> ();
			var controller = animator.runtimeAnimatorController as AnimatorController;
			if (!controller) {
				var path = Path.Combine (Path.GetDirectoryName (AssetDatabase.GetAssetPath (prefab)), prefab.name + ".controller");
				controller = AssetDatabase.LoadAssetAtPath<AnimatorController> (path);
				if (!controller) controller = AnimatorController.CreateAnimatorControllerAtPath (path);
				animator.runtimeAnimatorController = controller;
			}
			return controller;
		}

		private static void ProcessAnimations (SpriterAnimation[] animations, AnimatorController controller) {
			var unused = new HashSet<string> ();
			foreach (var animation in animations)
				unused.Add (animation.Name);
			foreach (var layer in controller.layers)
				foreach (var state in layer.stateMachine.states)
					if (unused.Contains (state.state.name)) {
						unused.Remove (state.state.name);
						ProcessState (state.state);
					}
			var machine = controller.layers [0].stateMachine;
			foreach (var animation in unused) {
				var state = machine.AddState (animation);
				ProcessState (state);
			}
		}

		private static void ProcessState (AnimatorState state) {
			if (!ArrayUtility.Find (state.behaviours, x => x is AnimatorRelay)) 
				state.AddStateMachineBehaviour<AnimatorRelay> ();
		}
	}
}
