using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform
{
    [TaskCategory("Basic/Transform")]
    [TaskDescription("Stores the position of the Transform. Returns Success.")]
    public class GetTranPosition : Action
    {
        [Tooltip("The Transform that the task operates on")]
        public SharedTransform targetTransform;
        [Tooltip("Can the target Transform be empty?")]
        [RequiredField]
        public SharedVector3 storeValue;


        //public override void OnStart()
        //{
        //    var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
        //    if (currentGameObject != prevGameObject) {
        //        targetTransform = currentGameObject.GetComponent<Transform>();
        //        prevGameObject = currentGameObject;
        //    }
        //}

        public override TaskStatus OnUpdate()
        {
            if (targetTransform == null) {
                Debug.LogWarning("Transform is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = targetTransform.Value.position;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetTransform = null;
            storeValue = Vector3.zero;
        }
    }
}