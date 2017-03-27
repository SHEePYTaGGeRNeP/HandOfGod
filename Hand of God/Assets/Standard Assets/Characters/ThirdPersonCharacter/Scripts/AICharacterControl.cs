namespace Assets.Standard_Assets.Characters.ThirdPersonCharacter.Scripts
{
    using UnityEngine;

    [RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target; // target to aim for

        // Use this for initialization
        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            this.agent = this.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            this.character = this.GetComponent<ThirdPersonCharacter>();

            this.agent.updateRotation = false;
            this.agent.updatePosition = true;
        }


        // Update is called once per frame
        private void Update()
        {
            if (this.target != null)
            {
                this.agent.SetDestination(this.target.position);

				
				
                // use the values to move the character
                this.character.Move(this.agent.desiredVelocity, false, false);
            }
            else
            {
                // We still need to call the character's move function, but we send zeroed input as the move param.
                this.character.Move(Vector3.zero, false, false);
            }

        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
