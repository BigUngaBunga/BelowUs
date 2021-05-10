using UnityEngine;
using Mirror;

namespace BelowUs
{
    public class TempFiriingscript : MonoBehaviour
    {
        //**Anv�nds endast f�r att testa skjua mot fiender, Tas bort sen**
        private float timeElapsed = 0;
        private Weapon weapon;
        private Transform firePoint;

        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private float timeBetweenShoots = 2;

        void Start()
        {
            weapon = GetComponent<Weapon>();
            firePoint = gameObject.transform.Find("FirePoint");
        }

        [Server]
        void Update()
        {
            UpdateFirePointRotation();
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= timeBetweenShoots)
            {
                timeElapsed -= timeBetweenShoots;
                weapon.Shoot();
            }
        }

        private void UpdateFirePointRotation()
        {
            Vector2 direction = targetGameObject.transform.position - firePoint.transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            firePoint.rotation = (Quaternion.Slerp(firePoint.transform.rotation, rotation, Time.deltaTime * 5f));
        }
    }
}
