using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace CardGame
{
    public class GridManager : MonoBehaviour
    {
        public GameObject cardPrefab;
        public int rows = 2;
        public int columns = 2;
        public float spacing = 10f;
        public float aspectRatio = 1.0f;

        void Start()
        {
            SetupGridLayout();
            CreateGrid(rows, columns);
        }
       
        public void CreateGrid(int rows, int columns)
        {

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            int totalCards = rows * columns;

            for (int i = 0; i < totalCards; i++)
            {
                    GameObject cardInstance = Instantiate(cardPrefab, transform);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void SetupGridLayout()
        {
            GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
            
            RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRect.sizeDelta;

            int padding = 10;
            gridLayoutGroup.padding = new RectOffset(padding, padding, padding, padding);

            float availableWidth = canvasSize.x - (padding * 2);
            float availableHeight = canvasSize.y - (padding * 2);

            float cellWidth = (availableWidth - (spacing * (columns - 1))) / columns;
            float cellHeight = (availableHeight - (spacing * (rows - 1))) / rows;

            if (cellWidth / cellHeight > aspectRatio)
            {
                cellWidth = cellHeight * aspectRatio;
            }
            else
            {
                cellHeight = cellWidth / aspectRatio;
            }

            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }

    }
}
