namespace MyGame.Script.Gameplay.Controller
{
    public interface ICharacterInteract
    {
        void OnInit();
        void OnDamageReceive();
        void OnFallDamageReceive();
    }
}
