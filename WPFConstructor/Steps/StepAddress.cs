using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConstructor
{
    public struct StepAddress
    {
        public StepAddress(int step = 0, int stage = 0)
        {
            Step = step;
            Stage = stage;
        }

        public static bool operator ==(StepAddress a1, StepAddress a2)
        {
            return a1.Stage == a2.Stage && a1.Step == a2.Step;
        }
        public static bool operator !=(StepAddress a1, StepAddress a2)
        {
            return a1.Stage != a2.Stage || a1.Step != a2.Step;

        }
        public int Step { get; }
        public int Stage { get; }
        public override string ToString()
        {
            return $"Этап {Stage+1}. Шаг {Step+1}.";
        }

        public override bool Equals(object obj)
        {
            return obj is StepAddress address &&
                   Step == address.Step &&
                   Stage == address.Stage;
        }

        public override int GetHashCode()
        {
            int hashCode = 2125369172;
            hashCode = hashCode * -1521134295 + Step.GetHashCode();
            hashCode = hashCode * -1521134295 + Stage.GetHashCode();
            return hashCode;
        }
    }
}
