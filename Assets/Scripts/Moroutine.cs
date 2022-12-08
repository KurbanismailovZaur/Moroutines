using Redcode.Moroutines.Exceptions;
using Redcode.Moroutines.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Redcode.Moroutines
{
    /// <summary>
    /// Represents a more advanced coroutine. You can control execution, subscribe to events, <br/>
    /// get the last result, wait for a specific events, and more.
    /// </summary>
    [Serializable]
    public sealed class Moroutine
    {
        #region Entities
        #region Awaiter classes
        /// <summary>
        /// Represents a base class for expectations.
        /// </summary>
        internal abstract class MoroutineAwaiter : YieldAwaiter
        {
            protected Moroutine _moroutine;

            /// <summary>
            /// Create awaiter-object which can await some moroutine events.
            /// </summary>
            /// <param name="moroutine">Moroutine, whose event is to be expected.</param>
            public MoroutineAwaiter(Moroutine moroutine) => _moroutine = moroutine;
        }

        /// <summary>
        /// Represents a class capable of waiting for a moroutine completion event.
        /// </summary>
        internal class CompleteAwaiter : MoroutineAwaiter
        {
            private IEnumerator _enumerator;

            /// <summary>
            /// Create awaiter-object which can await moroutine's complete event.
            /// </summary>
            /// <param name="moroutine"><inheritdoc cref="MoroutineAwaiter(Moroutine)"/></param>
            public CompleteAwaiter(Moroutine moroutine) : base(moroutine) => _enumerator = moroutine._enumerator;

            /// <summary>
            /// Should we continue to wait for the moroutine to be completed, or has it already been completed?
            /// </summary>
            public override bool KeepWaiting => _enumerator == _moroutine._enumerator && !(_moroutine.IsCompleted || _moroutine.IsDestroyed);
        }

        /// <summary>
        /// Represents a class capable of waiting for a moroutine stop event.
        /// </summary>
        internal class StopAwaiter : MoroutineAwaiter
        {
            private Coroutine _coroutine;

            /// <summary>
            /// Create awaiter-object which can await moroutine's stop event.
            /// </summary>
            /// <param name="moroutine"><inheritdoc cref="MoroutineAwaiter(Moroutine)"/></param>
            public StopAwaiter(Moroutine moroutine) : base(moroutine) => _coroutine = moroutine._coroutine;

            /// <summary>
            /// Should we continue to wait for the moroutine to be stopped, or has it already been stopped?
            /// </summary>
            public override bool KeepWaiting => _coroutine == _moroutine._coroutine && _moroutine.IsRunning;
        }

        /// <summary>
        /// Represents a class capable of waiting for a moroutine run event.
        /// </summary>
        internal class RunAwaiter : MoroutineAwaiter
        {
            private Coroutine _coroutine;

            /// <summary>
            /// Create awaiter-object which can await moroutine's run event.
            /// </summary>
            /// <param name="moroutine"><inheritdoc cref="MoroutineAwaiter(Moroutine)"/></param>
            public RunAwaiter(Moroutine moroutine) : base(moroutine) => _coroutine = moroutine._coroutine;

            /// <summary>
            /// Should we continue to wait for the moroutine to be run, or has it already been runned?
            /// </summary>
            public override bool KeepWaiting => _coroutine == _moroutine._coroutine && !_moroutine.IsRunning;
        }

        /// <summary>
        /// Represents a class capable of waiting for a moroutine reset event.
        /// </summary>
        internal class ResetAwaiter : MoroutineAwaiter
        {
            private IEnumerator _enumerator;

            /// <summary>
            /// Create awaiter-object which can await moroutine's reset event.
            /// </summary>
            /// <param name="moroutine"><inheritdoc cref="MoroutineAwaiter(Moroutine)"/></param>
            public ResetAwaiter(Moroutine moroutine) : base(moroutine) => _enumerator = moroutine._enumerator;

            /// <summary>
            /// Should we continue to wait for the moroutine to be reset, or has it already been reseted?
            /// </summary>
            public override bool KeepWaiting => _enumerator == _moroutine._enumerator;
        }

        /// <summary>
        /// Represents a class capable of waiting for a moroutine destroyed event.
        /// </summary>
        internal class DestroyAwaiter : MoroutineAwaiter
        {
            /// <summary>
            /// Create awaiter-object which can await moroutine's destroy event.
            /// </summary>
            /// <param name="moroutine"><inheritdoc cref="MoroutineAwaiter(Moroutine)"/></param>
            public DestroyAwaiter(Moroutine moroutine) : base(moroutine) { }

            /// <summary>
            /// Should we continue to wait for the moroutine to be destroyed, or has it already been destroyed?
            /// </summary>
            public override bool KeepWaiting => !_moroutine.IsDestroyed;
        }
        #endregion

        /// <summary>
        /// Represents the state of moroutine.
        /// </summary>
        [Flags]
        public enum State
        {
            /// <summary>
            /// Morutina is in the initial state.
            /// </summary>
            Reseted = 1,

            /// <summary>
            /// Morutina is being executed right now.
            /// </summary>
            Running = 2,

            /// <summary>
            /// Morutina is suspended.
            /// </summary>
            Stopped = 4,

            /// <summary>
            /// The execution of the moroutine is completed.
            /// </summary>
            Completed = 8,

            /// <summary>
            /// Moroutine completely destroyed.
            /// </summary>
            Destroyed = 16,
        }
        #endregion

        #region State
        /// <summary>
        /// Name of the moroutine.
        /// </summary>
        public string Name { get; set; }

        private State _state = State.Reseted;

        /// <summary>
        /// Current state of the  moroutine.
        /// </summary>
        public State CurrentState
        {
            get => _state;
            private set
            {
                _state = value;

                var stateEvent = _state switch
                {
                    State.Reseted => Reseted,
                    State.Running => Running,
                    State.Stopped => Stopped,
                    State.Completed => Completed,
                    State.Destroyed => Destroyed,
                    _ => throw new PlayControlException("Wrong moroutine state.")
                };

                stateEvent?.Invoke(this);
            }
        }

        /// <summary>
        /// Is moroutine in reset state?
        /// </summary>
        public bool IsReseted => CurrentState == State.Reseted;

        /// <summary>
        /// Is moroutine being performed right now?
        /// </summary>
        public bool IsRunning => CurrentState == State.Running;

        /// <summary>
        /// Is moroutine in stopped state?
        /// </summary>
        public bool IsStopped => CurrentState == State.Stopped;

        /// <summary>
        /// Did moroutine complete the fulfillment?
        /// </summary>
        public bool IsCompleted => CurrentState == State.Completed;

        /// <summary>
        /// Is moroutine destroyed?
        /// </summary>
        public bool IsDestroyed => CurrentState == State.Destroyed;

        /// <summary>
        /// Does the moroutine have an owner?
        /// </summary>
        public bool IsOwned { get; private set; }

        /// <summary>
        /// The last result of the moroutine (the last one that was returned via the yield return instruction inside moroutine). 
        /// </summary>
        public object LastResult => _enumerator?.Current;

        /// <summary>
        /// Is it need to destroy moroutine after completion? Auto destroy will be ignored if moroutine was created from <see cref="IEnumerator"/> object.
        /// </summary>
        public bool AutoDestroy { get; set; }
        #endregion

        #region Owner and routines
        /// <summary>
        /// Reference to the <see cref="Redcode.Moroutines.MoroutinesOwner"/> component of the owner object.
        /// </summary>
        public MoroutinesOwner Owner { get; private set; }

        private IEnumerable _enumerable;

        private IEnumerator _enumerator;

        private Coroutine _coroutine;
        #endregion

        #region Events
        /// <summary>
        /// The event that is redeemed when the moroutine resets to its initial state.
        /// </summary>
        public event Action<Moroutine> Reseted;

        /// <summary>
        /// The event that is redeemed when the moroutine starts performing.
        /// </summary>
        public event Action<Moroutine> Running;

        /// <summary>
        /// The event that is redeemed when the moroutine stops executing.
        /// </summary>
        public event Action<Moroutine> Stopped;

        /// <summary>
        /// The event that is redeemed when the moroutine complete executing.
        /// </summary>
        public event Action<Moroutine> Completed;

        /// <summary>
        /// The event that is emit when the moroutine destroyed.
        /// </summary>
        public event Action<Moroutine> Destroyed;
        #endregion

        private Moroutine(GameObject owner, IEnumerable enumerable)
        {
            _enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
            _enumerator = _enumerable.GetEnumerator();

            if (owner == null)
            {
                MoroutinesExecuter.Instance.Owner.Add(this);
                return;
            }

            Owner = owner.TryGetComponent(out MoroutinesOwner existingOwner) ? existingOwner : owner.AddComponent<MoroutinesOwner>();
            Owner.Add(this);

            IsOwned = true;
        }

        #region GetUnownedMoroutines
        /// <summary>
        /// Gets all unowned moroutines.<br/>
        /// Destroyed moroutines are not taken into account.
        /// </summary>
        /// <returns>Unowned moroutines.</returns>
        public static List<Moroutine> GetUnownedMoroutines() => MoroutinesExecuter.Instance.gameObject.GetMoroutines();

        /// <summary>
        /// Gets unowned moroutines by <paramref name="mask"/>.
        /// Destroyed moroutines are not taken into account.
        /// </summary>
        /// <param name="mask">State mask.</param>
        /// <returns>Unowned moroutines.</returns>
        public static List<Moroutine> GetUnownedMoroutines(State mask) => MoroutinesExecuter.Instance.gameObject.GetMoroutines(mask);
        #endregion

        #region Creation
        #region Single
        /// <summary>
        /// Create moroutine.
        /// </summary>
        /// <param name="enumerator">Enumerator which will be perform by moroutine. <br/>
        /// The <see cref="Reset"/> method call will be ignored.</param>
        /// <returns>The moroutine.</returns>
        public static Moroutine Create(IEnumerator enumerator) => Create(new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// <inheritdoc cref="Create(IEnumerator)"/>
        /// </summary>
        /// <param name="enumerable">Enumerable which will be perform by moroutine. <br/>
        /// Calling the <see cref="Reset"/> method will reset the state of the moroutine.</param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Create(IEnumerable enumerable) => Create((GameObject)null, enumerable);

        /// <summary>
        /// Create moroutine with owner.
        /// </summary>
        /// <param name="ownersComponent">The owner's component of the moroutine. Moroutine will be owned by the component's gameObject.</param>
        /// <param name="enumerator"><inheritdoc cref="Create(IEnumerator)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Create(Component ownersComponent, IEnumerator enumerator) => Create(ownersComponent, new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// Create moroutine with owner.
        /// </summary>
        /// <param name="ownersComponent"><inheritdoc cref="Create(Component, IEnumerator)" path="/param[@name='ownersComponent']"/></param>
        /// <param name="enumerable"><inheritdoc cref="Create(IEnumerable)" path="/param[@name='enumerable']"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Create(Component ownersComponent, IEnumerable enumerable) => Create(ownersComponent.gameObject, enumerable);

        /// <summary>
        /// Create moroutine with owner.
        /// </summary>
        /// <param name="owner">The owner of the moroutine.</param>
        /// <param name="enumerator"><inheritdoc cref="Create(IEnumerator)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Create(GameObject owner, IEnumerator enumerator) => Create(owner, new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// Create moroutine with owner.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator)" path="/param[@name='owner']"/></param>
        /// <param name="enumerable"><inheritdoc cref="Create(IEnumerable)" path="/param[@name='enumerable']"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Create(GameObject owner, IEnumerable enumerable) => new(owner, enumerable);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// <param name="enumerator"><inheritdoc cref="Create(IEnumerator)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(IEnumerator enumerator) => Run(new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// <param name="enumerable"><inheritdoc cref="Create(IEnumerable)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(IEnumerable enumerable) => Run((GameObject)null, enumerable);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// <param name="ownersComponent">The owner's component of the moroutine. Moroutine will be owned by the component's gameObject.</param>
        /// <param name="enumerator"><inheritdoc cref="Run(IEnumerator)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(Component ownersComponent, IEnumerator enumerator) => Run(ownersComponent, new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// /// <param name="ownersComponent"><inheritdoc cref="Run(Component, IEnumerator)" path="/param[@name='ownersComponent']"/></param>
        /// <param name="enumerable"><inheritdoc cref="Create(IEnumerable)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(Component ownersComponent, IEnumerable enumerable) => Run(ownersComponent.gameObject, enumerable);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator)" path="/param[@name='owner']"/></param>
        /// <param name="enumerator"><inheritdoc cref="Create(IEnumerator)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(GameObject owner, IEnumerator enumerator) => Run(owner, new EnumerableEnumerator(enumerator)).SetAutoDestroy(true);

        /// <summary>
        /// Create and run moroutine.
        /// </summary>
        /// /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator)" path="/param[@name='owner']"/></param>
        /// <param name="enumerable"><inheritdoc cref="Create(IEnumerable)"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator)"/></returns>
        public static Moroutine Run(GameObject owner, IEnumerable enumerable) => Create(owner, enumerable).Run();
        #endregion

        #region Multiple
        private static List<Moroutine> SetAutoDestroy(List<Moroutine> moroutines)
        {
            moroutines.ForEach(m => m.AutoDestroy = true);
            return moroutines;
        }

        private static List<Moroutine> Run(List<Moroutine> moroutines)
        {
            moroutines.ForEach(m => m.Run());
            return moroutines;
        }

        /// <summary>
        /// Create multiple moroutines.
        /// </summary>
        /// <param name="enumerators">Enumerators which will be perform by moroutines. <br/>
        /// The <see cref="Reset"/> method call will be ignored.</param>
        /// <returns>The moroutines.</returns>
        public static List<Moroutine> Create(params IEnumerator[] enumerators) => SetAutoDestroy(Create(enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// <inheritdoc cref="Create(IEnumerator[])"/>
        /// </summary>
        /// <param name="enumerables">Enumerables which will be perform by moroutines. <br/>
        /// Calling the <see cref="Reset"/> method will reset the state of the moroutine.</param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Create(params IEnumerable[] enumerables) => Create((GameObject)null, enumerables);

        /// <summary>
        /// Create moroutines with owner.
        /// </summary>
        /// <param name="ownersComponent">The owner's component of the moroutines. Moroutines will be owned by the component's gameObject.</param>
        /// <param name="enumerators"><inheritdoc cref="Create(IEnumerator[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Create(Component ownersComponent, params IEnumerator[] enumerators) => SetAutoDestroy(Create(ownersComponent, enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// Create moroutines with owner.
        /// </summary>
        /// <param name="ownersComponent"><inheritdoc cref="Create(Component, IEnumerator[])" path="/param[@name='ownersComponent']"/></param>
        /// <param name="enumerables"><inheritdoc cref="Create(IEnumerable[])" path="/param[@name='enumerables']"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Create(Component ownersComponent, params IEnumerable[] enumerables) => Create(ownersComponent.gameObject, enumerables);

        /// <summary>
        /// Create moroutines with owner.
        /// </summary>
        /// <param name="owner">The owner of the moroutines.</param>
        /// <param name="enumerators"><inheritdoc cref="Create(IEnumerator[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Create(GameObject owner, params IEnumerator[] enumerators) => SetAutoDestroy(Create(owner, enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// Create moroutines with owner.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator[])" path="/param[@name='owner']"/></param>
        /// <param name="enumerables"><inheritdoc cref="Create(IEnumerable[])" path="/param[@name='enumerables']"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Create(GameObject owner, params IEnumerable[] enumerables) => enumerables.Select(e => new Moroutine(owner, e)).ToList();

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// <param name="enumerators"><inheritdoc cref="Create(IEnumerator[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(params IEnumerator[] enumerators) => SetAutoDestroy(Run(enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// <param name="enumerables"><inheritdoc cref="Create(IEnumerable[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(params IEnumerable[] enumerables) => Run((GameObject)null, enumerables);

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// <param name="ownersComponent">The owner's component of the moroutines. Moroutines will be owned by the component's gameObject.</param>
        /// <param name="enumerators"><inheritdoc cref="Run(IEnumerator[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(Component ownersComponent, params IEnumerator[] enumerators) => SetAutoDestroy(Run(ownersComponent, enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// /// <param name="ownersComponent"><inheritdoc cref="Run(Component, IEnumerator[])" path="/param[@name='ownersComponent']"/></param>
        /// <param name="enumerables"><inheritdoc cref="Create(IEnumerable[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(Component ownersComponent, params IEnumerable[] enumerables) => Run(ownersComponent.gameObject, enumerables);

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator[])" path="/param[@name='owner']"/></param>
        /// <param name="enumerators"><inheritdoc cref="Create(IEnumerator[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(GameObject owner, params IEnumerator[] enumerators) => SetAutoDestroy(Run(owner, enumerators.Select(e => new EnumerableEnumerator(e)).ToArray()));

        /// <summary>
        /// Create and run moroutines.
        /// </summary>
        /// /// <param name="owner"><inheritdoc cref="Create(GameObject, IEnumerator[])" path="/param[@name='owner']"/></param>
        /// <param name="enumerables"><inheritdoc cref="Create(IEnumerable[])"/></param>
        /// <returns><inheritdoc cref="Create(IEnumerator[])"/></returns>
        public static List<Moroutine> Run(GameObject owner, params IEnumerable[] enumerables) => Run(Create(owner, enumerables));
        #endregion
        #endregion

        /// <summary>
        /// Sets passed name to moroutine.
        /// </summary>
        /// <param name="name">Name to set.</param>
        /// <returns>The moroutine.</returns>
        public Moroutine SetName(string name)
        {
            Name = name;
            return this;
        }

        #region Control
        /// <summary>
        /// Starts the moroutine for execution. If the moroutine has been stopped before, the method will continue.
        /// </summary>
        /// <param name="rerunIfCompleted">Is it necessary to automatically restart the moroutine if it has been completed?</param>
        /// <returns>The moroutine.</returns>
        /// <exception cref="PlayControlException"></exception>
        public Moroutine Run(bool rerunIfCompleted = true)
        {
            if (IsRunning)
                return this;

            if (IsDestroyed)
                throw new PlayControlException("Moroutine already destroyed.");

            if (IsOwned)
            {
                if (Owner == null)
                    throw new PlayControlException($"Moroutine couldn't be started because the game object's Owner component is missing.");

                if (!Owner.gameObject.activeInHierarchy)
                    throw new PlayControlException($"Moroutine couldn't be started because the game object '{Owner.name}' is deactivated.");
            }

            if (IsCompleted)
            {
                if (!rerunIfCompleted)
                    throw new PlayControlException("Moroutine was completed. If you want to restart it, call \"Reset\" method before.");

                Reset();
            }

            CurrentState = State.Running;
            _coroutine = MoroutinesExecuter.Instance.StartCoroutine(RunEnumerator());

            return this;
        }

        private IEnumerator RunEnumerator()
        {
            while (true)
            {
                if (!_enumerator.MoveNext())
                    break;

                yield return _enumerator.Current;
            }

            CurrentState = State.Completed;

            if (AutoDestroy)
                Destroy();
        }

        /// <summary>
        /// Stops the moroutine if it in playing state.
        /// </summary>
        /// <returns>The moroutine.</returns>
        /// <exception cref="PlayControlException"></exception>
        public Moroutine Stop()
        {
            if (!IsRunning)
                return this;

            if (_coroutine != null)
                MoroutinesExecuter.Instance.StopCoroutine(_coroutine);

            CurrentState = State.Stopped;

            return this;
        }

        /// <summary>
        /// Resets the moroutine to initial state.
        /// </summary>
        /// <returns>The moroutine</returns>
        /// <exception cref="PlayControlException"></exception>
        public Moroutine Reset()
        {
            if (IsReseted || IsDestroyed)
                return this;

            if (_coroutine != null)
                MoroutinesExecuter.Instance.StopCoroutine(_coroutine);

            _enumerator = _enumerable.GetEnumerator();
            CurrentState = State.Reseted;

            return this;
        }

        /// <summary>
        /// Shorthand for:
        /// <code>
        /// Reset().Run()
        /// </code>
        /// </summary>
        /// <returns>The moroutine.</returns>
        public Moroutine Rerun() => Reset().Run();
        #endregion

        #region Owning
        /// <summary>
        /// Sets owner to the moroutine.
        /// </summary>
        /// <param name="component">Component whose game object used as owner.</param>
        /// <returns>The moroutine.</returns>
        public Moroutine SetOwner(Component component) => SetOwner(component.gameObject);

        /// <summary>
        /// Sets owner to the moroutine.
        /// </summary>
        /// <param name="component">Owner for the moroutine.</param>
        /// <returns>The moroutine.</returns>
        public Moroutine SetOwner(GameObject gameObject)
        {
            if (IsDestroyed)
                throw new PlayControlException($"Destroyed moroutine can't change its owner.");

            if (!IsOwned && gameObject == null || IsOwned && Owner.gameObject == gameObject)
                return this;

            if (!IsOwned)
                MoroutinesExecuter.Instance.Owner.Remove(this);
            else
            {
                Owner.Remove(this);
                Owner.TryDestroy();
            }

            if (gameObject == null)
            {
                Owner = null;
                MoroutinesExecuter.Instance.Owner.Add(this);
                IsOwned = false;
            }
            else
            {
                Owner = gameObject.TryGetComponent(out MoroutinesOwner existingOwner) ? existingOwner : gameObject.AddComponent<MoroutinesOwner>();
                Owner.Add(this);
                IsOwned = true;

                if (!Owner.gameObject.activeInHierarchy)
                    Stop();
            }

            return this;
        }

        /// <summary>
        /// Make moroutine unowned.
        /// </summary>
        /// <returns>The moroutine.</returns>
        public Moroutine MakeUnowned() => SetOwner((GameObject)null);

        internal void OnOwnerDiactivate()
        {
            // MoroutinesExecuter can be null when we close the application. 
            if (MoroutinesExecuter.Instance == null)
                return;

            if (IsRunning)
                Stop();
        }
        #endregion

        #region Subscribing
        private Moroutine OnSubscribe(ref Action<Moroutine> ev, Action<Moroutine> action)
        {
            ev += action;
            return this;
        }

        /// <summary>
        /// Subscribe to reset event.
        /// </summary>
        /// <param name="action">Callback to invoke.</param>
        /// <returns>The moroutine.</returns>
        public Moroutine OnReseted(Action<Moroutine> action) => OnSubscribe(ref Reseted, action);

        /// <summary>
        /// Subscribe to run event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutine.</returns>
        public Moroutine OnRunning(Action<Moroutine> action) => OnSubscribe(ref Running, action);

        /// <summary>
        /// Subscribe to stop event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutine.</returns>
        public Moroutine OnStopped(Action<Moroutine> action) => OnSubscribe(ref Stopped, action);

        /// <summary>
        /// Subscribe to completed event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutine.</returns>
        public Moroutine OnCompleted(Action<Moroutine> action) => OnSubscribe(ref Completed, action);

        /// <summary>
        /// Subscribe to destroyed event.
        /// </summary>
        /// <param name="action"><inheritdoc cref="OnReseted(Action{Moroutine})"/></param>
        /// <returns>The moroutine.</returns>
        public Moroutine OnDestroyed(Action<Moroutine> action) => OnSubscribe(ref Destroyed, action);
        #endregion

        #region Yielders
        /// <summary>
        /// Create an awaiter object, wich knows how to wait until the moroutine is complete.
        /// </summary>
        /// <returns>Awaiter object.</returns>
        public YieldAwaiter WaitForComplete() => new CompleteAwaiter(this);

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until the moroutine is stopped.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete()"/></returns>
        public YieldAwaiter WaitForStop() => new StopAwaiter(this);

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until the moroutine is running.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete()"/></returns>
        public YieldAwaiter WaitForRun() => new RunAwaiter(this);

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until the moroutine is reseted.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete()"/></returns>
        public YieldAwaiter WaitForReset() => new ResetAwaiter(this);

        /// <summary>
        /// Create an awaiter object, wich knows how to wait until the moroutine is destroyed.
        /// </summary>
        /// <returns><inheritdoc cref="WaitForComplete()"/></returns>
        public YieldAwaiter WaitForDestroy() => new DestroyAwaiter(this);
        #endregion

        #region Destroying
        /// <summary>
        /// Stop and destroy moroutine immediatly. You can't run the moroutine after it destroying.
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
                return;

            if (IsRunning)
                Stop();

            if (!IsOwned)
                MoroutinesExecuter.Instance.Owner.Remove(this);
            else
            {
                Owner.Remove(this);
                Owner.TryDestroy();
                Owner = null;
            }

            CurrentState = State.Destroyed;
        }

        /// <summary>
        /// Sets moroutine's auto destroying.
        /// </summary>
        /// <param name="autoDestroy"><see langword="true"/> if you need destroy moroutine after completion.</param>
        /// <returns>The moroutine.</returns>
        public Moroutine SetAutoDestroy(bool autoDestroy)
        {
            AutoDestroy = autoDestroy;
            return this;
        }
        #endregion
    }
}