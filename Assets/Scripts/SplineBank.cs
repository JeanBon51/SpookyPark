using UnityEngine;

[CreateAssetMenu(menuName = "DataBank/SplineBank")]
public class SplineBank : ScriptableObject { 
	[SerializeField] private AutoSortingContainer[] _splinesPrefab;
	
	public AutoSortingContainer GetSplinePrefabByNames(int levelIndex) {
		if (this._splinesPrefab.Length <= levelIndex || levelIndex < 0) return null;
		return this._splinesPrefab[levelIndex];
	}
}
