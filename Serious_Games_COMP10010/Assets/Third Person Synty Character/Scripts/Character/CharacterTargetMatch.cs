using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTargetMatch : MonoBehaviour
{
    public List<MatchData> matchData = new List<MatchData> ();
   
    public void SetData(params MatchData[] datas)
    {
        matchData.Clear ();

        for (int i = 0; i < datas.Length; i++)
        {
            matchData.Add ( datas[i] );
        }
    }

    public MatchData CurrentData(float normalisedTime)
    {
        for (int i = 0; i < matchData.Count; i++)
        {
            if(normalisedTime >= matchData[i].start && normalisedTime <= matchData[i].end)
            {
                return matchData[i];
            }
        }

        return null;
    }

    public class MatchData
    {
        public MatchData (Vector3 targetPosition, Quaternion targetRotation, AvatarTarget targetBodyPath, Vector3 positionWeight, float rotationWeight, float start, float end)
        {
            this.targetPosition = targetPosition;
            this.targetRotation = targetRotation;
            this.targetBodyPath = targetBodyPath;
            this.positionWeight = positionWeight;
            this.rotationWeight = rotationWeight;
            this.start = start;
            this.end = end;
        }

        public Vector3 targetPosition { get; protected set; }
        public Quaternion targetRotation { get; protected set; }
        public AvatarTarget targetBodyPath { get; protected set; }
        public Vector3 positionWeight { get; protected set; }
        public float rotationWeight { get; protected set; }
        public float start { get; protected set; }
        public float end { get; protected set; }
    }

}
