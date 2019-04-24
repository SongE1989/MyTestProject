using UnityEngine;
using System.Collections;
namespace rtsNS
{
    public class TankMoveTest : MonoBehaviour
    {
        public Tank TheTank;
        public Plane ThePlane;
        public Transform firePos;
        public Transform fireTarget;
        public Transform awayPos;

        void Awake()
        {
            //TheTank.ToPosition(TarTran);
            ThePlane.ToPosition(firePos, fireTarget, awayPos);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ThePlane.ToPosition(firePos, fireTarget, awayPos);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                TheTank.ToPosition(firePos);
        }
    }
}