using LibreHardwareMonitor.Hardware;

namespace HardwareMonitor.Monitor
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        #region IVisitor

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();

            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            foreach (ISensor sensor in hardware.Sensors) sensor.Accept(this);
        }

        public void VisitSensor(ISensor sensor)
        {
            foreach (IParameter parameter in sensor.Parameters) parameter.Accept(this);
        }

        public void VisitParameter(IParameter parameter)
        {
        }

        #endregion IVisitor
    }
}
