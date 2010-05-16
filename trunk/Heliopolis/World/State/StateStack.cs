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

        public void AddListOfSubstates(IEnumerable<ActorState> statesToAdd)
        {
            foreach (var actorState in statesToAdd.Reverse())
            {
                _executingStateStack.Push(actorState);
            }
        }

        public void AddNewSubstate(ActorState newState)
        {
            _executingStateStack.Push(newState);
        }

        public string Tick()
        {
            bool keepProcessing = true;
            while (keepProcessing)
            {
                ActorState stateToProcess = _executingStateStack.Peek();

                if (!stateToProcess.Entered)
                {
                    stateToProcess.OnEnter();
                    stateToProcess.Entered = true;
                }
                else
                {
                    stateToProcess.Tick();
                    if (stateToProcess.Finished)
                    {
                        ActorState finishMe = _executingStateStack.Pop();
                        finishMe.OnFinish();
                    }
                }

                keepProcessing = !_executingStateStack.Peek().RequiresTime;
            }

            return _executingStateStack.Peek().ActionType;
        }
    }
}
