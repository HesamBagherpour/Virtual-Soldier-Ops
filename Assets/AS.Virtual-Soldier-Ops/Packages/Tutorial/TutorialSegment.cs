using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AS.Virtual_Soldier_Ops.Packages.Tutorial
{
    public class TutorialSegment : MonoBehaviour
    {
        public event Action<int> StepStarted;
        public event Action<int> StepPassed;
        public event Action<bool, int> TutorialStateChanged;
       
        [SerializeField] private List<TutorialStep> tutorialSteps;
        public int lastStartedStep;
         public int lastFinishedStep;
      

        public void ShowStep(int step, bool fromInit = false)
        {
            if (step <= lastFinishedStep && !fromInit) return;
            if(step>lastStartedStep+1) return;

            if (fromInit)
            {
                lastFinishedStep = step - 1;
            }
            var st = tutorialSteps.Find(s => s.Step == step);
            if (st != null) st.ShowStep();
            OnStepStarted(step);
        }

        public void HideStep(int step)
        {
            if (step <= lastFinishedStep) return;
            if(step>lastStartedStep+1) return;
            var st = tutorialSteps.Find(s => s.Step == step);
            if (st != null) st.HideStep();

            OnStepPassed(step);
        }

        public void SetStep(int lastStartedStep, int lastFinishedStep)
        {
            this.lastStartedStep = lastStartedStep;
            this.lastFinishedStep = lastFinishedStep;
        }

        protected virtual void OnStepPassed(int step)
        {
            lastFinishedStep = step;
            StepPassed?.Invoke(step);
            
            TutorialStateChanged?.Invoke(false, step);
           
        }

        protected virtual void OnStepStarted(int step)
        {
            lastStartedStep = step;
            TutorialStateChanged?.Invoke(true, step);
            StepStarted?.Invoke(step);
        }

        public void Init()
        {
            if (lastFinishedStep < lastStartedStep)
            {
                var step = FindLastStartableStep(lastStartedStep);
                ShowStep(step, true);
            }
        }

        public int FindLastStartableStep(int lastStep)
        {
            while (true)
            {
                if (lastStep == 1) return 1;

                if (tutorialSteps.Find(s => s.Step == lastStep).Startable)
                {
                    return lastStep;
                }

                lastStep -= 1;
            }
        }
    }

    public enum HighlightType
    {
        CirculateAroundObject,
        AnimatedArrow,
        AnimatedHand,
    }

}