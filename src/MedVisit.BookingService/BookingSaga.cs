namespace MedVisit.BookingService
{
    public class BookingSaga
    {
        private readonly List<SagaStep> _steps = new();
        private readonly Stack<SagaStep> _executedSteps = new();

        public void AddStep(string name, Func<Task<bool>> action, Func<Task> compensate)
        {
            _steps.Add(new SagaStep(name, action, compensate));
        }

        public async Task ExecuteAsync()
        {
            foreach (var step in _steps)
            {
                var success = await step.Action();
                if (!success)
                {
                    await CompensateAsync();
                    throw new Exception($"Шаг '{step.Name}' завершился с ошибкой. Откат изменений.");
                }
                _executedSteps.Push(step);
            }
        }

        private async Task CompensateAsync()
        {
            while (_executedSteps.Count > 0)
            {
                var step = _executedSteps.Pop();
                if (step.Compensate != null)
                {
                    await step.Compensate();
                }
            }
        }
    }

    public class SagaStep
    {
        public string Name { get; }
        public Func<Task<bool>> Action { get; }
        public Func<Task> Compensate { get; }

        public SagaStep(string name, Func<Task<bool>> action, Func<Task> compensate)
        {
            Name = name;
            Action = action;
            Compensate = compensate;
        }
    }

}
