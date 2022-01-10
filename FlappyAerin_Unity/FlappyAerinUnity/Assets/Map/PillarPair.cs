using Tools;
using UnityEngine;

public class PillarPair : MonoBehaviour, IReusable
{
    [SerializeField] MapMovementParams @params;
    [SerializeField] Transform top;
    [SerializeField] Transform bot;

    public void SetSeparation(float dist)
    {
        // Remember, each unit is 16 pixels. If you want a 64 pixel gap, dist should be 4
        top.localPosition = new Vector3(0, dist / 2, 0);
        bot.localPosition = new Vector3(0, -dist / 2, 0);
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.position += Vector3.left @params.speed * delta;
        if (transform.position.x > -1) return;
        gameObject.SetActive(false);
        ReturnToBag(this);
    }

    public ReturnDelegate ReturnToBag { get; set; }
}
