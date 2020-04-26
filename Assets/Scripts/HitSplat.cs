
using UnityEngine;

// Object exists for duration of the visual hitsplat.
public class HitSplat : RisingInfo { 
    public void init(int dmg, int state, Vector3 pos) {
        set_text(dmg.ToString());
        set_color(state);
        transform.position = pos;
    }

    private void set_color(int state) {
        if (state == Body.ALIVE)
            fg_T.color = BLUE;
        else if (state == Body.DEAD)
            fg_T.color = RED;
        else if (state == Body.INJURED)
            fg_T.color = ORANGE;
    }
}