public interface ITabStrategy
{
    /*
  Thinking will need a ToogleEvent which is triggered by a click event
  The toggle event brings to the next followup message
  */
    void Activate();
    void Deactivate();
}
