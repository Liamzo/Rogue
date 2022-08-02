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
        //Compute(new Vector2Int(parent.x, parent.y), parent.unitStats.stats[(int)Stats.Sight].GetValue());

        // Find new visible targets
        foreach (UnitController unit in game.units) {
            if (unit is PlayerController) {
                continue;
            }

            if (unit.spriteRenderer.gameObject.activeSelf == true) {
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

            // Inner loop: walk down the current column.  We start at the top, where x==y.
            //
            // TODO: we waste time walking across the entire column when the view area
            //   is narrow.  Experiment with computing the possible range of cells from
            //   the slopes, and iterate over that instead.
            for (int yc = currentCol; yc >= 0; yc--) {
                // Translate local coordinates to grid coordinates.  For the various octants
                // we need to invert one or both values, or swap x for y.
                int gridx = gridPosn.x + xc * txfrm.xx + yc * txfrm.xy;
                int gridy = gridPosn.y + xc * txfrm.yx + yc * txfrm.yy;

                // Range-check the values.  This lets us avoid the slope division for blocks
                // that are outside the grid.
                //
                // Note that, while we will stop at a solid column of blocks, we do always
                // start at the top of the column, which may be outside the grid if we're (say)
                // checking the first octant while positioned at the north edge of the map.
                if (gridx < 0 || gridx >= xDim || gridy < 0 || gridy >= yDim) {
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
                    map.SetLight(gridx, gridy, distanceSquared);
					visibleTiles.Add(new Vector2Int(gridx,gridy));
                }

                bool curBlocked = !map.CheckSight(gridx, gridy);

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


    #region New_FOV
    public void Compute(Vector2Int origin, int rangeLimit) {
        game.map.SetLight(origin.x, origin.y);
        visibleTiles.Add(origin);
        for(uint octant=0; octant<8; octant++) Compute(octant, origin, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1));
    }

    struct Slope // represents the slope y/x as a rational number
    {
        public Slope(uint y, uint x) { Y = y; X = x; }

        public bool Greater(uint y, uint x) { return Y*x > X*y; } // this > y/x
        public bool GreaterOrEqual(uint y, uint x) { return Y*x >= X*y; } // this >= y/x
        public bool Less(uint y, uint x) { return Y*x < X*y; } // this < y/x
        //public bool LessOrEqual(uint y, uint x) { return y*x <= x*y; } // this <= y/x

        public readonly uint X, Y;
    }

    void Compute(uint octant, Vector2Int origin, int rangeLimit, uint x, Slope top, Slope bottom)
    {
        // throughout this function there are references to various parts of tiles. a tile's coordinates refer to its
        // center, and the following diagram shows the parts of the tile and the vectors from the origin that pass through
        // those parts. given a part of a tile with vector u, a vector v passes above it if v > u and below it if v < u
        //    g         center:        y / x
        // a------b   a top left:      (y*2+1) / (x*2-1)   i inner top left:      (y*4+1) / (x*4-1)
        // |  /\  |   b top right:     (y*2+1) / (x*2+1)   j inner top right:     (y*4+1) / (x*4+1)
        // |i/__\j|   c bottom left:   (y*2-1) / (x*2-1)   k inner bottom left:   (y*4-1) / (x*4-1)
        //e|/|  |\|f  d bottom right:  (y*2-1) / (x*2+1)   m inner bottom right:  (y*4-1) / (x*4+1)
        // |\|__|/|   e middle left:   (y*2) / (x*2-1)
        // |k\  /m|   f middle right:  (y*2) / (x*2+1)     a-d are the corners of the tile
        // |  \/  |   g top center:    (y*2+1) / (x*2)     e-h are the corners of the inner (wall) diamond
        // c------d   h bottom center: (y*2-1) / (x*2)     i-m are the corners of the inner square (1/2 tile width)
        //    h
        for(; x <= (uint)rangeLimit; x++) // (x <= (uint)rangeLimit) == (rangeLimit < 0 || x <= rangeLimit)
        {
            // compute the y coordinates of the top and bottom of the sector. we maintain that top > bottom
            uint topy;
            if(top.X == 1) // if top == ?/1 then it must be 1/1 because 0/1 < top <= 1/1. this is special-cased because top
            {              // starts at 1/1 and remains 1/1 as long as it doesn't hit anything, so it's a common case
                topy = x;
            }
            else // top < 1
            {
                // get the tile that the top vector enters from the left. since our coordinates refer to the center of the
                // tile, this is (x-0.5)*top+0.5, which can be computed as (x-0.5)*top+0.5 = (2(x+0.5)*top+1)/2 =
                // ((2x+1)*top+1)/2. since top == a/b, this is ((2x+1)*a+b)/2b. if it enters a tile at one of the left
                // corners, it will round up, so it'll enter from the bottom-left and never the top-left
                topy = ((x*2-1) * top.Y + top.X) / (top.X*2); // the y coordinate of the tile entered from the left
                // now it's possible that the vector passes from the left side of the tile up into the tile above before
                // exiting from the right side of this column. so we may need to increment topy
                if(BlocksLight(x, topy, octant, origin)) // if the tile blocks light (i.e. is a wall)...
                {
                // if the tile entered from the left blocks light, whether it passes into the tile above depends on the shape
                // of the wall tile as well as the angle of the vector. if the tile has does not have a beveled top-left
                // corner, then it is blocked. the corner is beveled if the tiles above and to the left are not walls. we can
                // ignore the tile to the left because if it was a wall tile, the top vector must have entered this tile from
                // the bottom-left corner, in which case it can't possibly enter the tile above.
                //
                // otherwise, with a beveled top-left corner, the slope of the vector must be greater than or equal to the
                // slope of the vector to the top center of the tile (x*2, topy*2+1) in order for it to miss the wall and
                // pass into the tile above
                    if(top.GreaterOrEqual(topy*2+1, x*2) && !BlocksLight(x, topy+1, octant, origin)) topy++;
                    }
                else
                {
                    // since this tile doesn't block light, there's nothing to stop it from passing into the tile above, and it
                    // does so if the vector is greater than the vector for the bottom-right corner of the tile above. however,
                    // there is one additional consideration. later code in this method assumes that if a tile blocks light then
                    // it must be visible, so if the tile above blocks light we have to make sure the light actually impacts the
                    // wall shape. now there are three cases: 1) the tile above is clear, in which case the vector must be above
                    // the bottom-right corner of the tile above, 2) the tile above blocks light and does not have a beveled
                    // bottom-right corner, in which case the vector must be above the bottom-right corner, and 3) the tile above
                    // blocks light and does have a beveled bottom-right corner, in which case the vector must be above the
                    // bottom center of the tile above (i.e. the corner of the beveled edge).
                    // 
                    // now it's possible to merge 1 and 2 into a single check, and we get the following: if the tile above and to
                    // the right is a wall, then the vector must be above the bottom-right corner. otherwise, the vector must be
                    // above the bottom center. this works because if the tile above and to the right is a wall, then there are
                    // two cases: 1) the tile above is also a wall, in which case we must check against the bottom-right corner,
                    // or 2) the tile above is not a wall, in which case the vector passes into it if it's above the bottom-right
                    // corner. so either way we use the bottom-right corner in that case. now, if the tile above and to the right
                    // is not a wall, then we again have two cases: 1) the tile above is a wall with a beveled edge, in which
                    // case we must check against the bottom center, or 2) the tile above is not a wall, in which case it will
                    // only be visible if light passes through the inner square, and the inner square is guaranteed to be no
                    // larger than a wall diamond, so if it wouldn't pass through a wall diamond then it can't be visible, so
                    // there's no point in incrementing topy even if light passes through the corner of the tile above. so we
                    // might as well use the bottom center for both cases.
                    uint ax = x*2; // center
                    if(BlocksLight(x+1, topy+1, octant, origin)) ax++; // use bottom-right if the tile above and right is a wall
                    if(top.Greater(topy*2+1, ax)) topy++;
                }
            }

            uint bottomy;
            if(bottom.Y == 0) // if bottom == 0/?, then it's hitting the tile at y=0 dead center. this is special-cased because
            {                 // bottom.y starts at zero and remains zero as long as it doesn't hit anything, so it's common
                bottomy = 0;
            }
            else // bottom > 0
            {
                bottomy = ((x*2-1) * bottom.Y + bottom.X) / (bottom.X*2); // the tile that the bottom vector enters from the left
                // code below assumes that if a tile is a wall then it's visible, so if the tile contains a wall we have to
                // ensure that the bottom vector actually hits the wall shape. it misses the wall shape if the top-left corner
                // is beveled and bottom >= (bottomy*2+1)/(x*2). finally, the top-left corner is beveled if the tiles to the
                // left and above are clear. we can assume the tile to the left is clear because otherwise the bottom vector
                // would be greater, so we only have to check above
                if(bottom.GreaterOrEqual(bottomy*2+1, x*2) && BlocksLight(x, bottomy, octant, origin) &&
                    !BlocksLight(x, bottomy+1, octant, origin))
                {
                    bottomy++;
                }
            }

            // go through the tiles in the column now that we know which ones could possibly be visible
            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            for(uint y = topy; (int)y >= (int)bottomy; y--) // use a signed comparison because y can wrap around when decremented
            {
                Debug.Log("goop");
                Debug.Log(GetDistance((int)x, (int)y, origin));
                if(rangeLimit < 0 || GetDistance((int)x, (int)y, origin) <= rangeLimit) // skip the tile if it's out of visual range
                {
                    bool isOpaque = BlocksLight(x, y, octant, origin);
                    // every tile where topy > y > bottomy is guaranteed to be visible. also, the code that initializes topy and
                    // bottomy guarantees that if the tile is opaque then it's visible. so we only have to do extra work for the
                    // case where the tile is clear and y == topy or y == bottomy. if y == topy then we have to make sure that
                    // the top vector is above the bottom-right corner of the inner square. if y == bottomy then we have to make
                    // sure that the bottom vector is below the top-left corner of the inner square
                    bool isVisible =
                        isOpaque || ((y != topy || top.Greater(y*4-1, x*4+1)) && (y != bottomy || bottom.Less(y*4+1, x*4-1)));
                    // NOTE: if you want the algorithm to be either fully or mostly symmetrical, replace the line above with the
                    // following line (and uncomment the Slope.LessOrEqual method). the line ensures that a clear tile is visible
                    // only if there's an unobstructed line to its center. if you want it to be fully symmetrical, also remove
                    // the "isOpaque ||" part and see NOTE comments further down
                    // bool isVisible = isOpaque || ((y != topy || top.GreaterOrEqual(y, x)) && (y != bottomy || bottom.LessOrEqual(y, x)));
                    Debug.Log("boop");
                    if(isVisible) SetVisible(x, y, octant, origin);

                    // if we found a transition from clear to opaque or vice versa, adjust the top and bottom vectors
                    if(x != rangeLimit) // but don't bother adjusting them if this is the last column anyway
                    {
                        if(isOpaque)
                        {
                            if(wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column,
                            {                  // so adjust the bottom vector upward and continue processing it in the next column
                                // if the opaque tile has a beveled top-left corner, move the bottom vector up to the top center.
                                // otherwise, move it up to the top left. the corner is beveled if the tiles above and to the left are
                                // clear. we can assume the tile to the left is clear because otherwise the vector would be higher, so
                                // we only have to check the tile above
                                uint nx = x*2, ny = y*2+1; // top center by default
                                // NOTE: if you're using full symmetry and want more expansive walls (recommended), comment out the next line
                                if(BlocksLight(x, y+1, octant, origin)) nx--; // top left if the corner is not beveled
                                if(top.Greater(ny, nx)) // we have to maintain the invariant that top > bottom, so the new sector
                                {                       // created by adjusting the bottom is only valid if that's the case
                                    // if we're at the bottom of the column, then just adjust the current sector rather than recursing
                                    // since there's no chance that this sector can be split in two by a later transition back to clear
                                    if(y == bottomy) { bottom = new Slope(ny, nx); break; } // don't recurse unless necessary
                                    else Compute(octant, origin, rangeLimit, x+1, top, new Slope(ny, nx));
                                }
                                else // the new bottom is greater than or equal to the top, so the new sector is empty and we'll ignore
                                {    // it. if we're at the bottom of the column, we'd normally adjust the current sector rather than
                                    if(y == bottomy) return; // recursing, so that invalidates the current sector and we're done
                                }
                            }
                            wasOpaque = 1;
                        }

                        else
                        {
                            if(wasOpaque > 0) // if we found a transition from opaque to clear, adjust the top vector downwards
                            {
                                // if the opaque tile has a beveled bottom-right corner, move the top vector down to the bottom center.
                                // otherwise, move it down to the bottom right. the corner is beveled if the tiles below and to the right
                                // are clear. we know the tile below is clear because that's the current tile, so just check to the right
                                uint nx = x*2, ny = y*2+1; // the bottom of the opaque tile (oy*2-1) equals the top of this tile (y*2+1)
                                // NOTE: if you're using full symmetry and want more expansive walls (recommended), comment out the next line
                                if(BlocksLight(x+1, y+1, octant, origin)) nx++; // check the right of the opaque tile (y+1), not this one
                                // we have to maintain the invariant that top > bottom. if not, the sector is empty and we're done
                                if(bottom.GreaterOrEqual(ny, nx)) return;
                                top = new Slope(ny, nx);
                            }
                            wasOpaque = 0;
                        }
                    }
                }
            }

        
            // if the column didn't end in a clear tile, then there's no reason to continue processing the current sector
            // because that means either 1) wasOpaque == -1, implying that the sector is empty or at its range limit, or 2)
            // wasOpaque == 1, implying that we found a transition from clear to opaque and we recursed and we never found
            // a transition back to clear, so there's nothing else for us to do that the recursive method hasn't already. (if
            // we didn't recurse (because y == bottomy), it would have executed a break, leaving wasOpaque equal to 0.)
            if(wasOpaque != 0) break;
        }
    }

    // NOTE: the code duplication between BlocksLight and SetVisible is for performance. don't refactor the octant
    // translation out unless you don't mind an 18% drop in speed
    bool BlocksLight(uint x, uint y, uint octant, Vector2Int origin)
    {
        uint nx = (uint)origin.x, ny = (uint)origin.y;
        switch(octant)
        {
        case 0: nx += x; ny -= y; break;
        case 1: nx += y; ny -= x; break;
        case 2: nx -= y; ny -= x; break;
        case 3: nx -= x; ny -= y; break;
        case 4: nx -= x; ny += y; break;
        case 5: nx -= y; ny += x; break;
        case 6: nx += y; ny += x; break;
        case 7: nx += x; ny += y; break;
        }
        return !game.map.CheckSight((int)nx, (int)ny);
    }

    void SetVisible(uint x, uint y, uint octant, Vector2Int origin)
    {
        uint nx = (uint)origin.x, ny = (uint)origin.y;
        switch(octant)
        {
        case 0: nx += x; ny -= y; break;
        case 1: nx += y; ny -= x; break;
        case 2: nx -= y; ny -= x; break;
        case 3: nx -= x; ny -= y; break;
        case 4: nx -= x; ny += y; break;
        case 5: nx -= y; ny += x; break;
        case 6: nx += y; ny += x; break;
        case 7: nx += x; ny += y; break;
        }
        game.map.SetLight((int)nx, (int)ny);
        visibleTiles.Add(new Vector2Int((int)nx, (int)ny));
    }
    

    public float GetDistance(int x, int y, Vector2Int origin) {
        int dx = x - origin.x;
        int dy = y - origin.y;

        float dist = Mathf.Sqrt((dx * dx) + (dy * dy));

        return Mathf.Abs(dist);
    }

    #endregion

}
