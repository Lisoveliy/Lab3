using System;
using System.Threading;
using System.Threading.Tasks;
namespace Lab3
{
    public class Calculate
    {
        public Calculate(double startX, double endN)
        {
            this.startX = startX;
            this.endN = endN;
        }
        ~Calculate() { 
            CancelCalculations();
        }
        public void RunCalculations()
        {
            mainCancellationSource = new CancellationTokenSource();
            mainCancellation = mainCancellationSource.Token;
            InvokeCalculations(startX, 0, endN);
        }
        public void CancelCalculations()
        {
            mainCancellationSource.Cancel();
        }
        public void PauseCalculations()
        {
            isPaused = true;
        }
        public void ContinueCalculations()
        {
            isPaused = false;
        }
        public bool isPaused
        {
            get; private set;
        } = false;
        public bool isStarted
        {
            get; private set;
        } = false;
        private Task mainCalculateThread = Task.CompletedTask;
        private CancellationTokenSource mainCancellationSource = null!;
        private CancellationToken mainCancellation;
        //Events
        public EventHandler<CalculateEventArgs> CalculateStatusChanged = delegate { };
        public EventHandler CalculationsEnded = delegate { };

        private double startX { get; set; }
        private double endN { get; set; }
        private double _currentN;
        public double CurrentN
        {
            get
            {
                return _currentN;
            }
            private set
            {
                _currentN = value;
                CalculateStatusChanged.Invoke(this, new(_currentN, _currentSum));
            }
        }

        private double _currentSum;
        public double CurrentSum
        {
            get
            {
                return _currentSum;
            }
            private set
            {
                _currentSum = value;
                CalculateStatusChanged.Invoke(this, new(_currentN, _currentSum));
            }
        }
        private void InvokeCalculations(double startX, int startN, double endN)
        {
            isStarted = true;
            mainCalculateThread = Task.Run(() =>
            {
                CurrentSum = 0;
                CurrentN = endN;
                for (CurrentN = startN; CurrentN < endN; CurrentN++)
                {
                    while (isPaused && !mainCancellation.IsCancellationRequested);
                    if (mainCancellation.IsCancellationRequested)
                    {
                        break;
                    }
                    CurrentSum += Math.Pow(startX, 2 * CurrentN + 1) / (2 * CurrentN + 1);
                }
                CalculationsEnded.Invoke(this, new EventArgs());
                isStarted = false;
            }, mainCancellation);
        }
    }
    public class CalculateEventArgs : EventArgs
    {
        public double currentX { get; set; }
        public double currentN { get; set; }
        public double currentSum { get; set; }
        public CalculateEventArgs(double currentN, double currentSum)
        {
            this.currentN = currentN;
            this.currentSum = currentSum;
        }
    }
}
