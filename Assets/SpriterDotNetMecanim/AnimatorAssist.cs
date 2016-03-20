using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriterDotNetUnity;

namespace SpriterDotNetMecanim {
	/// <summary>
	/// Bridges communications between AnimatorController and UnitySpriterAnimator.
	/// </summary>
	[RequireComponent (typeof(SpriterDotNetBehaviour), typeof (Animator))]
	public class AnimatorAssist : MonoBehaviour {
		private Animator MecanimAnimator;
		private UnitySpriterAnimator SpriterAnimator;
		private Dictionary<int, string> Animations = new Dictionary<int, string> ();
		private Dictionary<int, AnimatorRelay> Relays = new Dictionary<int, AnimatorRelay> ();

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
		}

		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public void Transition (AnimatorRelay relay, int animationID, int layerID) {
//			if (Relays.ContainsKey (Animator.StringToHash ("hit_0"))) {
//				Debug.Log (Relays [Animator.StringToHash ("hit_0")].CustomTransitions [Animator.StringToHash ("idle")]);
//			}
			if (!Relays.ContainsKey (animationID)) {
				Relays.Add (animationID, relay);
			}
//			foreach (var rrelay in Relays.Values) {
//				Debug.Log (rrelay == relay);
//			}
			string animation;
			var length = GlobalTransitionDuration;
			if (MecanimAnimator.IsInTransition (layerID)) {
				if (MecanimAnimator.GetAnimatorTransitionInfo (layerID).anyState) {
				}
				else {
					var newLength = Relays [MecanimAnimator.GetCurrentAnimatorStateInfo (layerID).shortNameHash].CustomTransitions [animationID];
					if (newLength >= 0) length = newLength;
				}
			}
			if (Animations.TryGetValue (animationID, out animation))
				SpriterAnimator.Transition (animation, length);
			//Debug.Log (length);
		}

		public void TestThis (AnimatorRelay hitrelay) {
//			Debug.Log (hitrelay == Relays [Animator.StringToHash ("hit_0")]);
		}
	}
}
