using System;
using System.Collections.Generic;
using UnityEngine;

namespace CogSim
{
    public class AgentObject : MapObject
    {
        private int energy, satiety, hydration, heatCapacity,security;
        public int MAX = 100;
        public int visionRange = 10;
        public Behavior Behavior { get; private set; }
        public Memory Memory { get; private set; }
        public Senses Senses { get; private set; }
        public Homeostasis Homeostasis { get; private set; }
        public Foresight Foresight { get; private set; }
        public int Energy
        {
            get
            {
                return energy;
            }
            set
            {
                energy = value;
            }
        }

        public int Satiety
        {
            get
            {
                return satiety;
            }
            set
            {
                satiety = value;
            }
        }
        public int Hydration
        {
            get
            {
                return hydration;
            }
            set
            {
                hydration = value;
            }
        }

        public int HeatCapacity
        {
            get
            {
                return heatCapacity;
            }
            set
            {
                heatCapacity = value;
            }
        }

        public float HydrationF
        {
            get
            {
                return hydration / MAX;
            }
            set
            {
                hydration = (int)value * MAX;
            }
        }

        public float HeatCapacityF
        {
            get
            {
                return heatCapacity / MAX;
            }
            set
            {
                heatCapacity = (int)value * MAX;
            }
        }
        public float EnergyF
        {
            get
            {
                return energy / MAX;
            }
            set
            {
                energy = (int)value * MAX;
            }
        }

        public float SatietyF
        {
            get
            {
                return satiety / MAX;
            }
            set
            {
                satiety = (int)value * MAX;
            }
        }

        public AgentObjectComponent AgentComp => gameObject.GetComponent<AgentObjectComponent>();
        public SimulationManager Manager => Finder.Manager;
        public new string Name = "Agent";
        public new string Description = "Simulated organism";
        public AgentObject(GameObject gameObject, string id, Vector3Int position) : base(gameObject, id, position)
        {
            Energy = MAX;
            Hydration = MAX;
            Satiety = MAX;
            HeatCapacity = MAX;
            Behavior = new Behavior(this);
            Memory = new Memory(this);
            Senses = new Senses(this);
            Homeostasis = new Homeostasis(this);
        }
        public Act GetRandomAct()
        {
            Act[] acts = (Act[])Enum.GetValues(typeof(Act));
            int randomIndex = UnityEngine.Random.Range(0, acts.Length);
            return acts[randomIndex];
        }
        public void Move(Act newPosition)
        {
            AgentComp.MoveAction(newPosition);
        }
        public override void Upkeep() 
        {
            /*
             * take observation - Senses.GetAndUpdateObservation(current location)
             * Query prior expectation -> forsight.Expectation
             * calculate free energy -> Behavior.GetAndUpdateFreeEnergy()
             * behavior.Upkeep()
             *  checks for affordance update need       |
             *      check for Policy update need        |
             *          check for Strategy update need  |
             *              check for Behavior Update   | behavior.AnalyzeFreeEnergy
             *  predict possible actions 
             *  Choose best action and build expectation matrix 
             *  create memory of the upkeep, store expectation and observation matrix
             * 
             */
        }

        public override void Update()
        {
            base.Update();

            this.Behavior.IntendedAct = GetRandomAct();
            //gameObject.GetComponent<AgentObjectComponent>().MoveAction(Act.Right);
            Move(Behavior.IntendedAct);


        }

        public void Observation()
        {

        }
        public void Reflection()
        {

        }

        public Act ActiveInference()
        {

            return Act.Left;
        }
        public void DrawIntentionInGUI()
        {

        }
        public Vector3Int[] AdjacentTiles
        {
            get
            {
                return new Vector3Int[]
                {
                    Position + new Vector3Int(1,0,0),
                    Position + new Vector3Int(0,0,1),
                    Position + new Vector3Int(-1,0,0),
                    Position + new Vector3Int(0,0,-1)
                };
            }
        }

        public IEnumerable<Vector3Int> PathableTiles()
        {
            foreach (var ind in AdjacentTiles)
            {
                if (Manager.IsPathable(ind)) yield return ind;
            }
        }
    }
}
