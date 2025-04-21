using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tubes_Tahap1_KPL_kelompok3.Model;

namespace Tubes_Tahap1_KPL_kelompok3.Automata
{
    public class PelanggaranStateMachine
    {
        struct Transisi
        {
            public StatusPelanggaran prevState;
            public StatusPelanggaran nextState;
            public Trigger trigger;

            public Transisi(StatusPelanggaran prevState, StatusPelanggaran nextState, Trigger trigger)
            {
                this.prevState = prevState;
                this.nextState = nextState;
                this.trigger = trigger;
            }
        }

        Transisi[] transitions =
        {
            new Transisi(StatusPelanggaran.DILAPORKAN, StatusPelanggaran.DISETUJUI, Trigger.SETUJUI),
            new Transisi(StatusPelanggaran.DISETUJUI, StatusPelanggaran.DIBERI_SANKSI, Trigger.BERI_SANKSI),
            new Transisi(StatusPelanggaran.DIBERI_SANKSI, StatusPelanggaran.SELESAI, Trigger.SELESAIKAN)
        };

        public StatusPelanggaran currentState = StatusPelanggaran.DILAPORKAN;

        public StatusPelanggaran getNextState(StatusPelanggaran prevState, Trigger trigger)
        {
            foreach (var t in transitions)
            {
                if (t.prevState == prevState && t.trigger == trigger)
                    return t.nextState;
            }

            return prevState;
        }

        public void activate(Trigger trigger)
        {
            currentState = getNextState(currentState, trigger);
        }
    }
}
