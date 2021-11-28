/*
 *   Arduino PC Health Monitor (PC Companion app)
 *   Polls the hardware sensors for data and forwards them on to the arduino device
 *   Copyright (C) 2021 Daniel Neve
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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
