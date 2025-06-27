using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    private Health health;
    private UnitMove unitMove;
    private UnitSpin unitSpin;
    private UnitShoot unitShoot;
    private RagDollSpawner ragDollSpawner;
    private BaseUnitAction[] baseUnitActionArray;
    [SerializeField] private Transform hitPoint;
    [Header("ѡ��ͼ��")]
    [SerializeField] private GameObject selectedVisual;


    private GridPosition currentGridpostion;
    private GridPosition lastGridpostion ;

    [Header("�ж���")]
    [SerializeField] private int MAX_actionPoint = 2;
    [SerializeField] private int actionPoint = 2;
    public  Action OnActionPointChanged;

    [Header("��Ӫ")]
    [SerializeField] private bool isEnemy = false;

    [Header("���")]
    [SerializeField] private Transform ActionCameraTransform;

    public static Action<Unit> OnAnyUnitSpawned;
    public static Action<Unit> OnAnyUnitDead;


    private void Awake()
    {
        health = GetComponent<Health>();
        ragDollSpawner = GetComponent<RagDollSpawner>();
        baseUnitActionArray = GetComponents<BaseUnitAction>();
        LevelGrid.Instance().SetUnitAtGridPosition(this,LevelGrid.Instance().GetGridPosition(transform.position));
        ReSetAtionPoint();
    }

    public T GetUnitAction<T>() where T : BaseUnitAction
    {
        foreach(BaseUnitAction baseUnitAction in baseUnitActionArray)
        {
            if (baseUnitAction is T)
                return (T)baseUnitAction;
        }
            Debug.LogWarning("�޷���Ŀ��Unit�ҵ�������BaseAction");
            return null;
    }

    private void OnEnable()
    {
        UnitActionManager.Instance().OnSelectedUnitChanged += UpdateSelectedVisual;
        TurnSysterm.Instance().OnTurnCountChanged += TurnSysterm_OnTurnCountChanged;
        health.OnDead += Health_OnDead;

        UpdateSelectedVisual(this,EventArgs.Empty);
        OnAnyUnitSpawned.Invoke(this);
    }

    private void OnDisable()
    {
        UnitActionManager.Instance().OnSelectedUnitChanged -= UpdateSelectedVisual;
        TurnSysterm.Instance().OnTurnCountChanged -= TurnSysterm_OnTurnCountChanged;
        health.OnDead -= Health_OnDead;
    }
    void Update()
    {


        currentGridpostion = LevelGrid.Instance().GetGridPosition(transform.position);
        if(currentGridpostion != lastGridpostion)
        {

            LevelGrid.Instance().SwitchUnitFromGridPositionToGridPosition(this,lastGridpostion,currentGridpostion);
            lastGridpostion = currentGridpostion;
        }
    }




    public void DisAbleSelectedVisual()
    {
        selectedVisual.SetActive(false);
    }
    public void EnableSelectedVisual()
    {
        selectedVisual.SetActive(true);
    }

    private void UpdateSelectedVisual(object sender,EventArgs empty)
    {
        if(UnitActionManager.Instance().GetSelectedUnit() == this)
            EnableSelectedVisual();
        else
        {
            DisAbleSelectedVisual();
        }
    }


    public BaseUnitAction[] GetBaseUnitActionArray()
    {

        return baseUnitActionArray; 
    }

    public GridPosition GetGridPosition()
    {
        return currentGridpostion;
    }

    private void SpendActionPoint(int cost)
    {
        actionPoint -= cost;
        OnActionPointChanged?.Invoke();
    }

    public bool TrySpendActionPoint(int cost)
    {
        if(actionPoint - cost >=0)
        {
            SpendActionPoint(cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetActionPoint()
    {
        return actionPoint;
    }

    public void ReSetAtionPoint()
    {
        actionPoint = MAX_actionPoint;
        OnActionPointChanged.Invoke();
    }

    public void TurnSysterm_OnTurnCountChanged()
    {
        if(isEnemy&&TurnSysterm.Instance().GetIsEnemyTurn() || 
            !isEnemy && !TurnSysterm.Instance().GetIsEnemyTurn() )
        ReSetAtionPoint();
    }

    public bool GetIsEnemy()
    {
        return isEnemy;
    }
    public Transform GetHitPoint()
    {
        return hitPoint;
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    private void Health_OnDead()
    {
        ragDollSpawner.SpawnRagDoll();
        OnAnyUnitDead.Invoke(this);
        Destroy(gameObject);
    }

    public Transform GetActionCameraTransform()
    {
        return ActionCameraTransform;
    }

}
