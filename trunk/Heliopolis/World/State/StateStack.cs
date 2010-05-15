using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heliopolis.World.State
{
    public class StateStack : GameWorldObject
    {
        private Actor _myActor;

        public StateStack(Actor myActor, GameWorld owner, ActorState initialState)
            : base(owner)
        {
            _myActor = myActor;
            _executingStateStack.Push(initialState);
        }

        private readonly Stack<ActorState> _executingStateStack = new Stack<ActorState>();

        public void AddNewSubstate(ActorState newState)
        {
            _executingStateStack.Push(newState);
        }

        private void FinishLoop()
        {
            _executingStateStack.Peek().OnFinish();
            _executingStateStack.Pop();
            if (_executingStateStack.Peek().Finished)
            {
                FinishLoop(); //recursion arghgjhgjhghg
            }
        }

        public string Tick()
        {
            bool keepProcessing = true;
            while (keepProcessing)
            {
                ActorState stateToProcess = _executingStateStack.Peek();

                if (stateToProcess.Finished)
                {
                    FinishLoop();
                }
                else if (!stateToProcess.Entered)
                    stateToProcess.OnEnter();
                else
                {
                    stateToProcess.Tick();
                    if (stateToProcess.Finished)
                    {
                        FinishLoop();
                    }
                }

                keepProcessing = !_executingStateStack.Peek().RequiresTime;
            }

            return _executingStateStack.Peek().ActionType;
        }
    }
}
