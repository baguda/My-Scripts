using CogSim;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CogSim
{


    public struct SensoryMatrix
    {
        public Vector3Int position;
        public List<Vector3> toWalls;
        public List<Vector3> toEdges;
        public List<Vector3> toFood;
        public List<Vector3> toAgents;
        internal MatrixType type;
        public Vector3 bearing; // vector from position to 
        


        // Constructor to initialize the SensoryMatrix from an Observation
        public SensoryMatrix(Senses observation, Vector3Int location)
        {
            this.position = location;
            this.toWalls = MapUtility.RelativeVectorsTo(observation.AllVisibleWalls, observation.position).ToList();
            this.toEdges = MapUtility.RelativeVectorsTo(observation.AllVisibleEdges, observation.position).ToList();
            this.toFood = MapUtility.RelativeVectorsTo(observation.AllVisibleFood.Select(s => s.position).ToHashSet(), observation.position).ToList();
            this.toAgents = MapUtility.RelativeVectorsTo(observation.AllVisibleAgents.Select(s => s.position).ToHashSet(), observation.position).ToList();
            type =  MatrixType.Observation;
            bearing = observation.agentObject.Behavior.curAffordance.GetNextWaypoint - position;

        }

        public float BearingFreeEnergy() => bearing.magnitude;

        // Methods to calculate free energy statistics for all vectors
        public float FreeEnergyMax() => toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).Max();
        public float FreeEnergyMin() => toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).Min();
        public float FreeEnergyMean() => toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).Average();
        public float FreeEnergyMedian() => CalculateMedian(toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).ToArray());
        public float FreeEnergyMode() => CalculateMode(toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).ToArray());
        public float FreeEnergySum() => toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).Sum();

        // Methods to calculate free energy statistics for vectors in a specific cardinal direction
        public float FreeEnergyMax(Vector3 direction) => FreeEnergyInDirection(direction).Max();
        public float FreeEnergyMin(Vector3 direction) => FreeEnergyInDirection(direction).Min();
        public float FreeEnergyMean(Vector3 direction) => FreeEnergyInDirection(direction).Average();
        public float FreeEnergyMedian(Vector3 direction) => CalculateMedian(FreeEnergyInDirection(direction).ToArray());
        public float FreeEnergyMode(Vector3 direction) => CalculateMode(FreeEnergyInDirection(direction).ToArray());
        public float FreeEnergySum(Vector3 direction) => FreeEnergyInDirection(direction).Sum();

        private IEnumerable<float> FreeEnergyInDirection(Vector3 direction)
        {
            return toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents)
                .Where(v => Vector3.Angle(v, direction) < 45f) // Consider vectors within 45 degrees of the given direction
                .Select(v => v.magnitude);
        }

        public float FreeEnergyVariance()
        {
            var magnitudes = toWalls.Concat(toEdges).Concat(toFood).Concat(toAgents).Select(v => v.magnitude).ToArray();
            return CalculateVariance(magnitudes);
        }

        // Standard Deviation for all vectors
        public float FreeEnergyStandardDeviation() => Mathf.Sqrt(FreeEnergyVariance());

        // Variance for vectors in a specific direction
        public float FreeEnergyVariance(Vector3 direction)
        {
            var magnitudes = FreeEnergyInDirection(direction).ToArray();
            return CalculateVariance(magnitudes);
        }

        // Standard Deviation for vectors in a specific direction
        public float FreeEnergyStandardDeviation(Vector3 direction) => Mathf.Sqrt(FreeEnergyVariance(direction));

        // Helper method to calculate variance
        private float CalculateVariance(float[] values)
        {
            if (values.Length == 0) return 0f;
            float mean = values.Average();
            float sumOfSquares = values.Sum(val => (val - mean) * (val - mean));
            return sumOfSquares / values.Length;
        }

        // Helper method for calculating median
        private float CalculateMedian(float[] array)
        {
            if (array.Length == 0) return 0f;
            System.Array.Sort(array);
            int midpoint = array.Length / 2;
            return array.Length % 2 == 0
                ? (array[midpoint - 1] + array[midpoint]) / 2.0f
                : array[midpoint];
        }

        // Helper method for calculating mode (most frequent value)
        private float CalculateMode(float[] array)
        {
            if (array.Length == 0) return 0f;

            var frequencies = array.GroupBy(i => i)
                                   .OrderByDescending(grp => grp.Count())
                                   .Select(grp => grp.Key)
                                   .ToList();

            return frequencies[0]; // Return the first (most frequent) value
        }



        // Method to calculate free energy matrix
        public static SensoryMatrix ActualizeExpectation(SensoryMatrix observation, SensoryMatrix expectation)
        {
            SensoryMatrix freeEnergy = new SensoryMatrix();
            freeEnergy.position = observation.position; // Assuming position doesn't change

            freeEnergy.toWalls = CalculateFreeEnergy(observation.toWalls, expectation.toWalls);
            freeEnergy.toEdges = CalculateFreeEnergy(observation.toEdges, expectation.toEdges);
            freeEnergy.toFood = CalculateFreeEnergy(observation.toFood, expectation.toFood);
            freeEnergy.toAgents = CalculateFreeEnergy(observation.toAgents, expectation.toAgents);
            freeEnergy.bearing = observation.bearing - expectation.bearing;
            freeEnergy.type = MatrixType.FreeEnergy;
            return freeEnergy;
        }

        // Helper method to calculate free energy for a given set of vectors
        private static List<Vector3> CalculateFreeEnergy(List<Vector3> observed, List<Vector3> expected)
        {
            List<Vector3> result = new List<Vector3>();

            // Compare each observed vector to expected vectors
            foreach (var observedVector in observed)
            {
                Vector3 closestExpected = FindClosestVector(observedVector, expected);
                Vector3 difference = observedVector - closestExpected;

                // If there's a significant difference, we consider this as free energy
                if (difference.sqrMagnitude > 0.01f) // Adjust this threshold as needed
                {
                    result.Add(difference); // Adding the difference vector as free energy
                }
            }

            // Handle cases where an expected vector wasn't observed
            foreach (var expectedVector in expected)
            {
                if (!observed.Exists(v => Vector3.Distance(v, expectedVector) < 0.01f)) // Using a small threshold for equality
                {
                    result.Add(expectedVector * -1); // Negative vector to signify an "expected but unseen" event
                }
            }

            // Handle cases where an observed vector wasn't expected
            foreach (var observedVector in observed)
            {
                if (!expected.Exists(v => Vector3.Distance(v, observedVector) < 0.01f))
                {
                    result.Add(observedVector); // Positive vector to signify an "unexpected observation"
                }
            }

            return result;
        }

        // Helper method to find the closest vector in the expected list to an observed vector
        private static Vector3 FindClosestVector(Vector3 observed, List<Vector3> expected)
        {
            Vector3 closest = Vector3.zero;
            float minDistance = Mathf.Infinity;

            foreach (var expect in expected)
            {
                float distance = Vector3.Distance(observed, expect);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = expect;
                }
            }

            return closest;
        }

        public static SensoryMatrix PredictExpectation(SensoryMatrix observation, Vector3Int newLocation)
        {
            SensoryMatrix expectation = new SensoryMatrix();
            expectation.position = newLocation;

            // Reorient vectors to the new location
            expectation.toWalls = ReorientVectors(observation.toWalls, observation.position, newLocation);
            expectation.toEdges = ReorientVectors(observation.toEdges, observation.position, newLocation);
            expectation.toFood = ReorientVectors(observation.toFood, observation.position, newLocation);
            expectation.toAgents = ReorientVectors(observation.toAgents, observation.position, newLocation);
            expectation.bearing = ReorientVector(observation.bearing, observation.position, newLocation);
            expectation.type = MatrixType.Expectation;
            return expectation;
        }

        // Helper method to reorient vectors from an old position to a new position
        private static List<Vector3> ReorientVectors(List<Vector3> vectors, Vector3Int oldPosition, Vector3Int newPosition)
        {
            List<Vector3> reorientedVectors = new List<Vector3>();

            foreach (var vector in vectors)
            {
                // Calculate the absolute position that the vector was pointing to from the old position
                Vector3 absolutePosition = oldPosition + vector;

                // Calculate the new relative vector from the new position to that absolute position
                Vector3 newVector = absolutePosition - newPosition;

                reorientedVectors.Add(newVector);
            }

            return reorientedVectors;
        }
        private static Vector3 ReorientVector(Vector3 vector, Vector3Int oldPosition, Vector3Int newPosition)
        {
            // Calculate the absolute position that the vector was pointing to from the old position
            Vector3 absolutePosition = oldPosition + vector;

            // Calculate the new relative vector from the new position to that absolute position
            return absolutePosition - newPosition;
        }
        internal enum MatrixType
        {
            Observation = 0,
            Expectation = 1,
            FreeEnergy = 2
        }
    }
}