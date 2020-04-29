 using UnityEngine;
 
 
 public class StateMachineBehaviour<T> : StateMachineBehaviour where T : Component
 {
     
     protected T Context { get; private set; }
     
     protected Animator Animator { get; private set; }
     
     
     protected Transform Transform { get; private set; }
     private bool _initialized;
     public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     {
         if (!_initialized)
         {
             Animator = animator;
             Context = animator.GetComponentInParent<T>();
             if (Context == null)
                 throw new System.InvalidOperationException(
                     $"State machine behaviour needs sibling/parent component of type {typeof(T)}");
             Transform = Context.transform;
             _initialized = true;
             OnInitialize(animator, stateInfo, layerIndex);
         }
         OnStateEntered(animator, stateInfo, layerIndex);
     }
     
     protected virtual void OnInitialize(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     { }
     
     protected virtual void OnStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     { }
 }