using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : Vision
{
    List<Vector2Int> visibleTiles = new List<Vector2Int>();

    public List<UnitController> visibleTargets = new List<UnitController>();

    public int currentIndex = -1;
    public GameObject currentTargetHighlight;
    public event System.Action TargetUnitChange;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        parent.OnTurnStart += OnTurnStartEnd;
        parent.OnTurnEnd += OnTurnStartEnd;
        currentTargetHighlight.SetActive(false);
    }

    void OnTurnStartEnd() {
        foreach (Vector2Int v in visibleTiles) {
			game.map.SetLightOff(v.x,v.y);
		}
		visibleTiles.Clear();
        visibleTargets.Clear();

		ComputeVisibility(game.map, new Vector2Int(parent.x, parent.y), parent.unitStats.stats[(int)Stats.Sight].GetValue() + 0.5f);

        // Find new visible targets
        foreach (UnitController unit in game.units) {
            if (unit is PlayerController) {
                continue;
            }

            if (unit.spriteRenderer.enabled == true) {
                visibleTargets.Add(unit);
            }
        }

        visibleTargets.Sort((a,b) => ((a.x - parent.x) + (a.y - parent.y)).CompareTo(((b.x - parent.x) + (b.y - parent.y))));

        if (currentIndex == -1 && visibleTargets.Count > 0) {
            UpdateTargetUnit(0);
        } else if (visibleTargets.Count == 0) {
            UpdateTargetUnit(-1);
        } else if (visibleTargets.Contains(currentTarget) == false) {
            if (visibleTargets.Count == 0) {
                UpdateTargetUnit(-1);
            } else {
                UpdateTargetUnit(0);
            }
        } else if (visibleTargets.Contains(currentTarget) == true) {
            UpdateTargetUnit(visibleTargets.IndexOf(currentTarget));
        }
    }

    public override UnitController FindTarget() {
        // TODO: Cycle through visible targets using Tab
        return currentTarget;
    }

    public override void ChangeTargetUnit(UnitController unit) {
        if (unit == null) {
            UpdateTargetUnit(-1);
        } else if (visibleTargets.Contains(unit)) {
            UpdateTargetUnit(visibleTargets.IndexOf(unit));
        } else {
            Debug.LogError("Clicked unit that was on screen but not in list of visibleUnits");
        }
    }

    public void CheckTargetInput() {
        if (Input.GetMouseButtonDown(0)) {
			// if (EventSystem.current.IsPointerOverGameObject()) {
			// 	return null;
			// }

			Tile tile = game.map.GetTileUnderMouse();

            if (tile != null) {
                if (tile.occupiedBy is EnemyController) {
                    if (visibleTargets.Contains((UnitController)tile.occupiedBy)) {
                        UpdateTargetUnit(visibleTargets.IndexOf((UnitController)tile.occupiedBy));
                    } else {
                        Debug.LogError("Clicked unit that was on screen but not in list of visibleUnits");
                    }
                }
            } else {
				UpdateTargetUnit(-1);
			}
		}

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (visibleTargets.Count > 0) {
                UpdateTargetUnit((currentIndex + 1) % visibleTargets.Count);
            } else {
                UpdateTargetUnit(-1);
            }
        }
    }

    public void UpdateTargetUnit(int index) {
        currentIndex = index;

        if (currentIndex == -1) {
            currentTarget = null;
            currentTargetHighlight.SetActive(false);
            currentTargetHighlight.transform.SetParent(this.transform, false);
        } else {
            currentTarget = visibleTargets[currentIndex];
            currentTargetHighlight.SetActive(true);
            currentTargetHighlight.transform.SetParent(currentTarget.transform, false);
        }

        TargetUnitChange();
    }

    #region FOV
	
	/// <summary>
    /// Immutable class for holding coordinate transform constants.  Bulkier than a 2D
    /// array of ints, but it's self-formatting if you want to log it while debugging.
    /// </summary>
    private class OctantTransform {
        public int xx { get; private set; }
        public int xy { get; private set; }
        public int yx { get; private set; }
        public int yy { get; private set; }

        public OctantTransform(int xx, int xy, int yx, int yy) {
            this.xx = xx;
            this.xy = xy;
            this.yx = yx;
            this.yy = yy;
        }

        public override string ToString() {
            // consider formatting in constructor to reduce garbage
            return string.Format("[OctantTransform {0,2:D} {1,2:D} {2,2:D} {3,2:D}]",
                xx, xy, yx, yy);
        }
    }
    private static OctantTransform[] s_octantTransform = {
        new OctantTransform( 1,  0,  0,  1 ),   // 0 E-NE
        new OctantTransform( 0,  1,  1,  0 ),   // 1 NE-N
        new OctantTransform( 0, -1,  1,  0 ),   // 2 N-NW
        new OctantTransform(-1,  0,  0,  1 ),   // 3 NW-W
        new OctantTransform(-1,  0,  0, -1 ),   // 4 W-SW
        new OctantTransform( 0, -1, -1,  0 ),   // 5 SW-S
        new OctantTransform( 0,  1, -1,  0 ),   // 6 S-SE
        new OctantTransform( 1,  0,  0, -1 ),   // 7 SE-E
    };

	/// <summary>
    /// Lights up cells visible from the current position.  Clear all lighting before calling.
    /// </summary>
    /// <param name="map">The cell grid definition.</param>
    /// <param name="gridPosn">The player's position within the grid.</param>
    /// <param name="viewRadius">Maximum view distance; can be a fractional value.</param>
    public void ComputeVisibility(Map map, Vector2Int gridPosn, float viewRadius) {
        //Debug.Assert(gridPosn.x >= 0 && gridPosn.x < grid.xDim);
        //Debug.Assert(gridPosn.y >= 0 && gridPosn.y < grid.yDim);

        // Viewer's cell is always visible.
        map.SetLight(gridPosn.x, gridPosn.y, 0.0f);
		visibleTiles.Add(gridPosn);

        // Cast light into cells for each of 8 octants.
        //
        // The left/right inverse slope values are initially 1 and 0, indicating a diagonal
        // and a horizontal line.  These aren't strictly correct, as the view area is supposed
        // to be based on corners, not center points.  We only really care about one side of the
        // wall at the edges of the octant though.
        //
        // NOTE: depending on the compiler, it's possible that passing the octant transform
        // values as four integers rather than an object reference would speed things up.
        // It's much tidier this way though.
        for (int txidx = 0; txidx < s_octantTransform.Length; txidx++) {
            CastLight(map, gridPosn, viewRadius, 1, 1.0f, 0.0f, s_octantTransform[txidx]);
        }

		// Check if items are on a visible tile?
    }

	/// <summary>
    /// Recursively casts light into cells.  Operates on a single octant.
    /// </summary>
    /// <param name="map">The cell grid definition.</param>
    /// <param name="gridPosn">The player's position within the grid.</param>
    /// <param name="viewRadius">The view radius; can be a fractional value.</param>
    /// <param name="startColumn">Current column; pass 1 as initial value.</param>
    /// <param name="leftViewSlope">Slope of the left (upper) view edge; pass 1.0 as
    ///   the initial value.</param>
    /// <param name="rightViewSlope">Slope of the right (lower) view edge; pass 0.0 as
    ///   the initial value.</param>
    /// <param name="txfrm">Coordinate multipliers for the octant transform.</param>
    ///
    /// Maximum recursion depth is (Ceiling(viewRadius)).
    private void CastLight(Map map, Vector2Int gridPosn, float viewRadius, int startColumn, float leftViewSlope, float rightViewSlope, OctantTransform txfrm) {
        //Debug.Assert(leftViewSlope >= rightViewSlope);

        // Used for distance test.
        float viewRadiusSq = viewRadius * viewRadius;

        int viewCeiling = Mathf.CeilToInt(viewRadius);

        // Set true if the previous cell we encountered was blocked.
        bool prevWasBlocked = false;

        // As an optimization, when scanning past a block we keep track of the
        // rightmost corner (bottom-right) of the last one seen.  If the next cell
        // is empty, we can use this instead of having to compute the top-right corner
        // of the empty cell.
        float savedRightSlope = -1;

        int xDim = map.width;
        int yDim = map.height;

        // Outer loop: walk across each column, stopping when we reach the visibility limit.
        for (int currentCol = startColumn; currentCol <= viewCeiling; currentCol++) {
            int xc = currentCol;

            // Inner loop: walk down the current column.  We start at the top, where X==Y.
            //
            // TODO: we waste time walking across the entire column when the view area
            //   is narrow.  Experiment with computing the possible range of cells from
            //   the slopes, and iterate over that instead.
            for (int yc = currentCol; yc >= 0; yc--) {
                // Translate local coordinates to grid coordinates.  For the various octants
                // we need to invert one or both values, or swap X for Y.
                int gridX = gridPosn.x + xc * txfrm.xx + yc * txfrm.xy;
                int gridY = gridPosn.y + xc * txfrm.yx + yc * txfrm.yy;

                // Range-check the values.  This lets us avoid the slope division for blocks
                // that are outside the grid.
                //
                // Note that, while we will stop at a solid column of blocks, we do always
                // start at the top of the column, which may be outside the grid if we're (say)
                // checking the first octant while positioned at the north edge of the map.
                if (gridX < 0 || gridX >= xDim || gridY < 0 || gridY >= yDim) {
                    continue;
                }

                // Compute slopes to corners of current block.  We use the top-left and
                // bottom-right corners.  If we were iterating through a quadrant, rather than
                // an octant, we'd need to flip the corners we used when we hit the midpoint.
                //
                // Note these values will be outside the view angles for the blocks at the
                // ends -- left value > 1, right value < 0.
                float leftBlockSlope = (yc + 0.5f) / (xc - 0.5f);
                float rightBlockSlope = (yc - 0.5f) / (xc + 0.5f);

                // Check to see if the block is outside our view area.  Note that we allow
                // a "corner hit" to make the block visible.  Changing the tests to >= / <=
                // will reduce the number of cells visible through a corner (from a 3-wide
                // swath to a single diagonal line), and affect how far you can see past a block
                // as you approach it.  This is mostly a matter of personal preference.
                if (rightBlockSlope >= leftViewSlope) {
                    // Block is above the left edge of our view area; skip.
                    continue;
                } else if (leftBlockSlope <= rightViewSlope) {
                    // Block is below the right edge of our view area; we're done.
                    break;
                }

                // This cell is visible, given infinite vision range.  If it's also within
                // our finite vision range, light it up.
                //
                // To avoid having a single lit cell poking out N/S/E/W, use a fractional
                // viewRadius, e.g. 8.5.
                //
                // TODO: we're testing the middle of the cell for visibility.  If we tested
                //  the bottom-left corner, we could say definitively that no part of the
                //  cell is visible, and reduce the view area as if it were a wall.  This
                //  could reduce iteration at the corners.
                float distanceSquared = xc * xc + yc * yc;
                if (distanceSquared <= viewRadiusSq) {
                    map.SetLight(gridX, gridY, distanceSquared);
					visibleTiles.Add(new Vector2Int(gridX,gridY));
                }

                bool curBlocked = !map.CheckSight(gridX, gridY);

                if (prevWasBlocked) {
                    if (curBlocked) {
                        // Still traversing a column of walls.
                        savedRightSlope = rightBlockSlope;
                    } else {
                        // Found the end of the column of walls.  Set the left edge of our
                        // view area to the right corner of the last wall we saw.
                        prevWasBlocked = false;
                        leftViewSlope = savedRightSlope;
                    }
                } else {
                    if (curBlocked) {
                        // Found a wall.  Split the view area, recursively pursuing the
                        // part to the left.  The leftmost corner of the wall we just found
                        // becomes the right boundary of the view area.
                        //
                        // If this is the first block in the column, the slope of the top-left
                        // corner will be greater than the initial view slope (1.0).  Handle
                        // that here.
                        if (leftBlockSlope <= leftViewSlope) {
                            CastLight(map, gridPosn, viewRadius, currentCol + 1,
                                leftViewSlope, leftBlockSlope, txfrm);
                        }

                        // Once that's done, we keep searching to the right (down the column),
                        // looking for another opening.
                        prevWasBlocked = true;
                        savedRightSlope = rightBlockSlope;
                    }
                }
            }

            // Open areas are handled recursively, with the function continuing to search to
            // the right (down the column).  If we reach the bottom of the column without
            // finding an open cell, then the area defined by our view area is completely
            // obstructed, and we can stop working.
            if (prevWasBlocked) {
                break;
            }
        }
    }

    #endregion
}
