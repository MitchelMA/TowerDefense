using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTower : MonoBehaviour
{
    [SerializeField] protected int baseDamage;
    [SerializeField] protected int baseSpeed;
    
    // Xp stats
    protected int currentXp;
    // Xp needed to lvl up
    protected int neededXp;
    // Xp increment to the neededXp after leveling up
    protected int xpIncOnLvlUp;

    private int _lvlPoints;
    public int LvlPoints => _lvlPoints;
    
    // Start is called before the first frame update
    public abstract void Start();

    // Update is called once per frame
    public abstract void Update();

    protected abstract void LvlUpCallBack();

    /// <summary>
    /// Adds the given XP amount to the current xp.
    /// Also checks if the tower can level-up
    /// </summary>
    /// <param name="xp"></param>
    /// <returns>The amount of levels the tower went up after gaining the xp</returns>
    public int XpUp(int xp)
    {
        this.currentXp += xp;
        return CheckLvlUp();
    }

    /// <summary>
    /// Checks if the Tower can level-up
    /// </summary>
    /// <returns>The amount of levels the tower gained</returns>
    private int CheckLvlUp()
    {
        int lvlups = 0;
        while (currentXp >= neededXp)
        {
            lvlups++;
            currentXp -= neededXp;
            neededXp += xpIncOnLvlUp;
            // call the lvlup callback
            LvlUpCallBack();
        }

        return lvlups;
    }
}
