/*
* @Author: 教忠言
* @Description:
* @Date: 2024年04月24日 星期三 13:04:02
* @Modify:
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 2.5f, RunSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;
    private float _verticalVelocity;
    public float Gravity = -9.8f;
    private Animator _animator;

    public bool IsPlayer = true;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;
    private Vector3 RandomPos = Vector3.zero;

    public float attackStartTime;
    public float AttackSlideDuration = 0.2f;
    public float AttackSlideSpeed = 0.1f;

    private Health _health;
    private DamageCaster _damageCaster;

    public SkinnedMeshRenderer _skinMashRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    private Vector3 impactOnCharacter;
    
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit,
    }

    private CharacterState CurrentState;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        if (IsPlayer)
            _playerInput = GetComponent<PlayerInput>();
        else
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
        }
    }

    private void CalculatePlayerMovement()
    {
        if (_playerInput.MouseButtonDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        
        _movementVelocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        
        _animator.SetBool("Shift", _playerInput.ShiftKeyDown);
        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _animator.SetBool("AirBorne", !_cc.isGrounded);

        if(_playerInput.ShiftKeyDown)
            _movementVelocity *=  RunSpeed * Time.deltaTime;
        else
            _movementVelocity *=  MoveSpeed * Time.deltaTime;

        if(_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
    }

    private void CalculateEnemyMovement()
    {
        if (TargetPlayer.gameObject.GetComponent<Health>().CurrentHealth <= 0)
        {
            if (RandomPos == Vector3.zero)
            {
                _navMeshAgent.SetDestination(RandomPos);
                _animator.SetFloat("Speed", 0.05f);
            }
            if(Vector3.Distance(RandomPos, transform.position) >= _navMeshAgent.stoppingDistance) return;
            float randomX = Random.Range(-13f, 8f);
            float randomZ = Random.Range(-11f, 2f);
            RandomPos = new Vector3(randomX, transform.position.y, randomZ);
            _navMeshAgent.SetDestination(RandomPos);
            _animator.SetFloat("Speed", 0.05f);
            return;
        }
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);
            
            SwitchStateTo(CharacterState.Attacking);
        }
        
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (IsPlayer)
                {
                    CalculatePlayerMovement();
                }
                else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (IsPlayer)
                {
                    
                    if (Time.time < attackStartTime + AttackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity = Vector3.Lerp( transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                if (impactOnCharacter.magnitude > 0.2f)
                {
                    _movementVelocity = impactOnCharacter * Time.deltaTime;
                    impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);
                }
                break;
        }
        if (IsPlayer)
        {
            if (!_cc.isGrounded)
                _verticalVelocity = Gravity;
            else
                _verticalVelocity = Gravity * 0.3f;
        
            _movementVelocity += Vector3.up * (_verticalVelocity * Time.deltaTime);
        
            _cc.Move(_movementVelocity);
            
            _movementVelocity = Vector3.zero;
        }
    }

    public void SwitchStateTo(CharacterState newState)
    {
        if(IsPlayer)
            _playerInput.MouseButtonDown = false;
        
        //Exiting State
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(_damageCaster != null)
                    DisableDamageCaster();
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
        }
        
        //Entering State
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                if (!IsPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                
                _animator.SetTrigger("Attack");
                
                if(IsPlayer)
                    attackStartTime = Time.time;
                
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                break;
        }

        CurrentState = newState;
        // Debug.Log("当前状态： " + CurrentState);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackPos = new Vector3())
    {
        if (_health != null)
        {
            _health.ApplyDamage(damage);
            StartCoroutine(MaterialLight());
        }

        if (!IsPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHit(attackPos);
        }
        if(IsPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackPos, 10f);
        }
    }

    public void AddImpact(Vector3 attackPos, float force)
    {
        Vector3 impactDir = transform.position - attackPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialLight()
    {
        _materialPropertyBlock.SetFloat("_blink", 1f);
        _materialPropertyBlock.SetColor("Blink_Color", Color.yellow);
        _skinMashRenderer.SetPropertyBlock(_materialPropertyBlock);
        yield return new WaitForSeconds(0.2f);
        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinMashRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2f);
        float dissolveTimeDirection = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeight_target = -12f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinMashRenderer.SetPropertyBlock(_materialPropertyBlock);
        while (currentDissolveTime < dissolveTimeDirection)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeight_target,
                currentDissolveTime / dissolveTimeDirection);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinMashRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        
    }
}
