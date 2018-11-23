using UnityEngine;
using System.Collections.Generic;
using Bounce.StateMachine;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Holds the state of the current level.
    /// </summary>
	public class Level : MonoBehaviour, IStateTransitioner, IConsumerListener
	{
        /// <summary>
        /// Level ids should be unique.
        /// </summary>
        public int levelId;

        /// <summary>
        /// List of shapes that have been hit by the player.
        /// </summary>
		public List<ConsumableShapeControl> m_ConsumedShapes { get; private set; }
        
        /// <summary>
        /// Indicates whether this level has been completed.
        /// </summary>
		public bool completed { get; set; }

		private ConsumableShapeControl[] m_ConsumableShapes;
        private NonConsumableShapeControl[] m_NonConsumableShapes;
	    private FSM m_StateMachine;
        private ShapeAudioCollection m_ConsumableShapeAudio;
        private ShapeAudioCollection m_NonConsumableShapeAudio;

	    void Awake()
	    {
	        m_StateMachine = GetComponent<FSM>();
			m_ConsumedShapes = new List<ConsumableShapeControl>();

			m_ConsumableShapes = GetComponentsInChildren<ConsumableShapeControl>();
            m_NonConsumableShapes = GetComponentsInChildren<NonConsumableShapeControl>();
	    }
			
        /// <summary>
        /// Adds shape to the list of hit shapes.
        /// </summary>
        /// <param name="shape"></param>
		public void AddShape( ConsumableShapeControl shape )
		{
            if( !m_ConsumedShapes.Contains( shape ) )
            {
                m_ConsumedShapes.Add( shape );
            }
		}

		public void ClearConsumedShapes()
		{
			m_ConsumedShapes.Clear();

            m_ConsumableShapeAudio.Reset();
            m_NonConsumableShapeAudio.Reset();
		}

		public bool AllConsumed()
		{
			return m_ConsumedShapes.Count == m_ConsumableShapes.Length;
		}
		
		public void DoUpdate ()
	    {
	        m_StateMachine.CurrentState.Reason();
	        m_StateMachine.CurrentState.Act();
	    }

        /// <summary>
        /// Setups audio, activates level object, and constructs the FSM.
        /// </summary>
        /// <param name="levelManager">The level manager responsible for this level.</param>
        /// <param name="consumableAudioClips">List of clips to play when shape is hit.</param>
        /// <param name="nonConsumableAudioClips">List of clips to play when a non consumable shape is hit.<</param>
		public void Enter( ILevelManager levelManager, AudioClip[] consumableAudioClips, AudioClip[] nonConsumableAudioClips )
		{
            gameObject.SetActive( true );

            m_ConsumableShapeAudio = new ShapeAudioCollection( consumableAudioClips, m_ConsumableShapes.Length );
            m_NonConsumableShapeAudio = new ShapeAudioCollection( nonConsumableAudioClips, m_NonConsumableShapes.Length );

            ClearConsumedShapes();
			ConstructFSM( levelManager );
            AddShapeListeners();

		}

        /// <summary>
        /// Called when level exits, disables level object.
        /// </summary>
		public void Exit()
		{
            gameObject.SetActive( false );
        }

	    /// <summary>
	    /// This performs a transistion from one state to another and is invoked in an individual states Reason method.
	    /// </summary>
	    public void SetTransistion( FSMTransistion transition )
	    {
	        m_StateMachine.PerformTransition( transition );
	    }

		private void OnConsumableShapeHit( ConsumableShapeControl shape )
		{

			Events.instance.Raise( new SFXAudioEvent( m_ConsumableShapeAudio.GetCurrentClip() ) );

            m_ConsumableShapeAudio.IncrementIndex();

			AddShape( shape );

			if( AllConsumed() )
			{
				Events.instance.Raise( new AllShapesConsumedEvent() );
			}
		}

        private void OnNonConsumableShapeHit(NonConsumableShapeControl shape)
        {
            Events.instance.Raise( new SFXAudioEvent( m_NonConsumableShapeAudio.GetCurrentClip() ) );
            m_NonConsumableShapeAudio.IncrementIndex();
        }

        private void AddShapeListeners()
		{
			foreach( var shape in m_ConsumableShapes )
			{
				shape.AddListener( OnConsumableShapeHit );
			}

            foreach( var shape in m_NonConsumableShapes )
            {
                shape.AddListener( OnNonConsumableShapeHit );
            }
		}


		private void ConstructFSM( ILevelManager levelManager )
		{
            m_StateMachine.ClearStates();

            // Initial state.
            var entryReason = new LevelEntryReason( FSMTransistion.Entry, this, true, FSMStateID.Start );
			var entryAction = new ShowShapesAction( GetAllShapes() );
			var entryState = new State( FSMStateID.Entry, entryAction, entryReason );
			m_StateMachine.AddState( entryState );

			// Start state.
			var startReason = new PlayerStatusChangeReason( this, PlayerStatus.Fired, FSMStateID.InProgress );
			var startAction = new IdleAction();
			var startState = new State( FSMStateID.Start, startAction, startReason );
			m_StateMachine.AddState( startState );

			// Moving state.
			var movingReasons = new List<FSMReason>();
			movingReasons.Add( new PlayerStatusChangeReason( this, PlayerStatus.Placed, FSMStateID.InProgress ) );
			movingReasons.Add( new PlayerOffScreenWithShapesRemainingReason( FSMTransistion.BallOutOfPlay, 
				this, this, true, FSMStateID.InProgress ) ); 
			movingReasons.Add( new PlayerOffScreenWithShapesRemainingReason ( FSMTransistion.AllConsumableShapesConsumed, 
				this, this, false, FSMStateID.Complete ) );
			movingReasons.Add( new ConsumptionShapesClearedReason( this, this, FSMStateID.Complete ) );
			var movingAction = new ShowConsumedShapesAction( this );
			var movingState = new State( FSMStateID.InProgress, movingAction, movingReasons );
			m_StateMachine.AddState( movingState );

			// Complete state.
			var completeState = new State( FSMStateID.Complete, new LevelCompleteAction( levelManager, m_NonConsumableShapes ), 
				new LevelEntryReason( FSMTransistion.Exit, this, false, FSMStateID.Entry ) );
			m_StateMachine.AddState( completeState );
		}

        private List<ShapeControl> GetAllShapes()
        {
            List<ShapeControl> shapes = new List<ShapeControl>();
            shapes.AddRange( m_ConsumableShapes );
            shapes.AddRange( m_NonConsumableShapes );
            return shapes;
        }
	}
}