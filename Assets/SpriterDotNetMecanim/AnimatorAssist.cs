using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SpriterDotNetUnity;

namespace SpriterDotNetMecanim {
	/// <summary>
	/// Bridges communications between AnimatorController and UnityAnimator.
	/// </summary>
	[RequireComponent (typeof(SpriterDotNetBehaviour), typeof (Animator))]
	public class AnimatorAssist : MonoBehaviour {
		public const string AnimDoneTrigger = "Animation_Finished";

		private Animator MecanimAnimator;
		private UnityAnimator SpriterAnimator;
		private Dictionary<int, string> Animations = new Dictionary<int, string> ();
		private Dictionary<int, AnimatorRelay> Relays = new Dictionary<int, AnimatorRelay> ();
		private AnyStateRelay[] AnyStates = new AnyStateRelay [0];

		/// <summary>
		/// The time it takes in milliseconds to transition from one animation to another.
		/// </summary>
		[Tooltip ("The time it takes in milliseconds to transition from one animation to another.")] 
		public float GlobalTransitionDuration = 500f;

		private IEnumerator Start () {
			MecanimAnimator = GetComponent<Animator> ();
			var behaviour = GetComponent<SpriterDotNetBehaviour> ();
			while (behaviour.Animator == null) yield return null;
			SpriterAnimator = behaviour.Animator;
			foreach (var animation in SpriterAnimator.GetAnimations ())
				Animations.Add (Animator.StringToHash (animation), animation);
			SpriterAnimator.AnimationFinished += AnimFinished;
		}

		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public void Transition (AnimatorRelay relay, int animationID, int layerID) {
			if (!Relays.ContainsKey (animationID)) {
				Relays.Add (animationID, relay);
			}
			string animation;
			var length = GlobalTransitionDuration;
			if (MecanimAnimator.IsInTransition (layerID)) {
				var newLength = -1f;
				if (MecanimAnimator.GetAnimatorTransitionInfo (layerID).anyState) newLength = AnyStates [layerID].CustomTransitions [animationID];
				else newLength = Relays [MecanimAnimator.GetCurrentAnimatorStateInfo (layerID).shortNameHash].CustomTransitions [animationID];
				if (newLength >= 0) length = newLength;
			}
			if (Animations.TryGetValue (animationID, out animation))
				if (length == 0) SpriterAnimator.Play (animation);
				else SpriterAnimator.Transition (animation, length);
		}

		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public void RegisterAnyState (AnyStateRelay relay, int layerID) {
			if (layerID >= AnyStates.Length) Array.Resize (ref AnyStates, layerID + 1);
			if (!AnyStates [layerID]) AnyStates [layerID] = relay;
		}

		private void AnimFinished (string info) {
			MecanimAnimator.SetTrigger (AnimDoneTrigger);
		}
	}
}
