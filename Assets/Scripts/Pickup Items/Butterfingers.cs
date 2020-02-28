namespace PickupItem
{
    public class Butterfingers : UseItem
    {
        public override void Use()
        {
            PlayerTransform.GetComponent<PlayerControl.PlayerPickupUse>().PickupPutdown();
        }
    }
}