using UnityEngine;

namespace MadLab.Utilities
{
    /// <summary>
    /// Adding comments to GameObjects in the Inspector.
    /// </summary>
    public class ML_ErrorComment : MonoBehaviour
    {
		/// <summary>
		/// The comment.
		/// </summary>
		[Multiline]
		public string text;
    }
}
