using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs
{
    public class FlipSubmarineComponent : MonoBehaviour
    {
        [SerializeField] private bool isSprite;
        private new SpriteRenderer renderer;

        private void Start() => renderer = isSprite ? GetComponent<SpriteRenderer>() : null;

        public void FlipObject(bool flipObject)
        {
            if (isSprite)
            {
                renderer.flipX = flipObject;
                transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            }
            else
                transform.localEulerAngles = flipObject ? -new Vector3(0, 180, 0) : Quaternion.identity.eulerAngles;
        }
    }
}