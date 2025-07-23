namespace Game_Manager.Conditions
{
    public interface IPollableCondition
    {
        /// <summary>
        /// Called every frame by the GameManager if the condition implements this interface.
        /// Useful for conditions that need to be checked continuously,
        /// such as input checks or time-based conditions.
        /// </summary>
        void OnUpdate();
    }
}