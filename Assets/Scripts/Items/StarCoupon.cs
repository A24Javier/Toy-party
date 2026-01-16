using UnityEngine;

[CreateAssetMenu(fileName = "StarCoupon.asset", menuName = ("Item functions/Create StarCoupon"))]
public class StarCoupon : ItemFunction
{
    public override void UseItem()
    {
        UIManager.instance.ControlStarCouponMsg(true);
    }

    public void DestroyItem()
    {
        // Quitar objeto del inventario del player
        item.inventoryAssociated.DeleteItem(item);
    }
}
