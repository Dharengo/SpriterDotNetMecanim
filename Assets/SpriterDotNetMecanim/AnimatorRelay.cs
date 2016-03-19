using UnityEngine;

namespace SpriterDotNetMecanim {
	/// <summary>
	/// Sends messages from the AnimatorController to the AnimatorAssist component.
	/// </summary>
	public class AnimatorRelay : StateMachineBehaviour {
		/// <summary>
		/// Not meant to be called manually.
		/// </summary>
		public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			var assistant = animator.GetComponent<AnimatorAssist> ();
			if (assistant) assistant.Transition (stateInfo.shortNameHash, layerIndex);
		}
	}
}
