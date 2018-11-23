using UnityEngine;
using System;
using System.Linq;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Manages updating and transitioning between levels.
    /// </summary>
	public class LevelManager : MonoBehaviour, ILevelManager
	{
		public LevelNumberAnimator levelNumberAnimator;
        public AudioEffectLibrary audioLibrary;

        private static readonly string CONSUME_AUDIO_GROUP_NAME = "piano_keys";
        private static readonly string NON_CONSUME_AUDIO_GROUP_NAME = "drums";
        private static readonly int LEVEL_COMPLETED_KEY = 1;
		private static readonly int KEY_NOT_FOUND = -1;

        private Level m_CurrentLevel { get { return m_Levels[ m_CurrentLevelIndex ]; } }
		private int m_CurrentLevelIndex;

        private AudioClip[] m_ConsumableShapeAudioClips;
        private AudioClip[] m_NonConsumableAudioClips;

		private Level[] m_Levels;
		public Level[] levels { get { return m_Levels; } }

        public Action OnNewLevelLoaded;

		private int m_HighestCompletedLevelID = 1;

		private bool m_ShouldUpdate;

        void Awake()
        {
			m_Levels = GetComponentsInChildren<Level> ();

			m_Levels.OrderBy( level => level.levelId);
        }

		void Start () 
		{
			m_ShouldUpdate = true;
			
			m_ConsumableShapeAudioClips = audioLibrary.GetGroupWithName( CONSUME_AUDIO_GROUP_NAME );
            m_NonConsumableAudioClips = audioLibrary.GetGroupWithName( NON_CONSUME_AUDIO_GROUP_NAME );

			foreach( var level in m_Levels )
			{
				SetLevelCompletedIfCompleteKeyExists( level );

				if( level.completed )
				{
					m_HighestCompletedLevelID = level.levelId;
				}
			}

			m_CurrentLevelIndex = m_HighestCompletedLevelID == 1 ? 0 : m_HighestCompletedLevelID;

			m_CurrentLevel.Enter( this, m_ConsumableShapeAudioClips, m_NonConsumableAudioClips );
		}

		void OnEnable()
		{
			Events.instance.AddListener<MenuStatusChangedEvent>( OnMenuStatusChanged );
		}

		void OnDisable()
		{
			Events.instance.RemoveListener<MenuStatusChangedEvent>( OnMenuStatusChanged );
		}
		

		void Update () 
		{
			if( m_ShouldUpdate )
			{
				m_CurrentLevel.DoUpdate();
			}
		}

		public void LoadLevel( int levelId )
		{
			Debug.Log ("Load level: " + levelId);

			MoveToLevel( levelId );
		}
			
		public void OnLevelComplete()
		{
			int newLevelIndex = m_CurrentLevelIndex + 1;

			if( newLevelIndex >= m_Levels.Length )
			{
				newLevelIndex = 0;
			}

			levelNumberAnimator.StartAnimation( m_CurrentLevelIndex + 1, 
				newLevelIndex + 1, OnLevelNumberAnimationComplete );

			StoreLevelCompleteForCurrentLevel();

			m_CurrentLevel.completed = true;

			if( m_CurrentLevel.levelId > m_HighestCompletedLevelID )
			{
				m_HighestCompletedLevelID = m_CurrentLevel.levelId;
			}
        }

		public int GetHighestCompletedLevelID()
		{
			return m_HighestCompletedLevelID;	
		}

        public bool IsLevelComplete(int levelId)
        {
            int levelIdToIndex = levelId - 1;

            if (!IsValidLevelID( levelIdToIndex ) )
            {
                return false;
            }

            return m_Levels[ levelIdToIndex ].completed;
        }

        private void OnLevelNumberAnimationComplete()
		{
			IncrementLevel();

			if (OnNewLevelLoaded != null )
			{
				OnNewLevelLoaded();
			}
		}

		private void StoreLevelCompleteForCurrentLevel( )
		{
			PlayerPrefs.SetInt( m_CurrentLevel.levelId.ToString(), LEVEL_COMPLETED_KEY );
		}

		private int LoadCompletedKeyForLevel( Level level )
		{
			return PlayerPrefs.GetInt( level.levelId.ToString (), KEY_NOT_FOUND );
		}

		private bool DoesCompletedKeyExistForLevel( Level level )
		{
			var key = LoadCompletedKeyForLevel( level );

			return key == LEVEL_COMPLETED_KEY;
		}

		private void SetLevelCompletedIfCompleteKeyExists( Level level )
		{
			if( DoesCompletedKeyExistForLevel( level ) )
			{
				level.completed = true;
			}
		}

		private void IncrementLevel()
		{
			int newLevelIndex = m_CurrentLevelIndex + 1;

			if( newLevelIndex >= m_Levels.Length )
			{
				DebugLog.instance.ApendLog( "Complete!" );
				newLevelIndex = 0;
			}

			MoveToLevel( newLevelIndex );
		}

		private void MoveToLevel( int levelID )
		{
			if( IsValidLevelID (levelID) )
			{
				m_CurrentLevel.Exit();

				m_CurrentLevelIndex = levelID;

				m_CurrentLevel.Enter( this, m_ConsumableShapeAudioClips, m_NonConsumableAudioClips );
			}
		}

		private bool IsValidLevelID( int levelID )
		{
			return levelID >= 0 && levelID < m_Levels.Length;
		}

		private void OnMenuStatusChanged( MenuStatusChangedEvent e )
		{
			bool playing = true;
			
			if( e.isAdditionalVisible )
			{
				playing = false;
			}

			m_ShouldUpdate = playing;
			m_CurrentLevel.gameObject.SetActive( playing );


		}
	}
}
