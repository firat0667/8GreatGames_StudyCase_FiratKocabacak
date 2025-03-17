using System;

namespace GreatGames.CaseLib.Key
{
    public interface IBasicSignal
    {
        public abstract void Connect(Action a_action);
        public abstract void Disconnect(Action a_action);
        public abstract void Emit();
    }

    public class BasicSignal : IBasicSignal
    {
        private Action m_action;


        public virtual void Connect(Action a_action)
        {
            m_action += a_action;
        }

        public void Disconnect(Action a_action)
        {
            m_action -= a_action;
        }

        public void Emit()
        {
            m_action?.Invoke();
        }

        public void DisconnectAll()
        {
            m_action = null;
        }
    }

    public class BasicSignal<T> : BasicSignal
    {
        private Action<T> m_action;

        public virtual void Connect(Action<T> a_action)
        {
            m_action += a_action;
        }

        public void Disconnect(Action<T> a_action)
        {
            m_action -= a_action;
        }

        public void Emit(T a_parameter)
        {
            m_action?.Invoke(a_parameter);
        }
    }

    public class BasicSignal<T1, T2> : BasicSignal
    {
        private Action<T1, T2> m_action;


        public void Connect(Action<T1, T2> a_action)
        {
            m_action += a_action;
        }

        public void Disconnect(Action<T1, T2> a_action)
        {
            m_action -= a_action;
        }

        public void Emit(T1 a_first, T2 a_second)
        {
            m_action?.Invoke(a_first, a_second);
        }
    }


    public class EmitOnlySignal<T> : BasicSignal<T>
    {
        public EmitOnlySignal(Action<T> action)
        {
            base.Connect(action);
        }

        public override void Connect(Action<T> a_action)
        {
            return;
        }
    }

    /// <summary>
    /// This signal can't be connected outside it's constructor.
    /// </summary>
    public class EmitOnlySignal : BasicSignal
    {
        public EmitOnlySignal(Action action)
        {
            base.Connect(action);
        }

        public override void Connect(Action a_action)
        {
            return;
        }
    }
}
