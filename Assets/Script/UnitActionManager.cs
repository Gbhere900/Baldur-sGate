using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionManager : MonoBehaviour
{
    static private UnitActionManager instance;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Unit selectedUnit;
    private BaseUnitAction selectedUniAction;
    private bool isBusy  = false;
    public Action OnActionCompeleted;
    public event EventHandler<bool> OnIsBusyChanged; 

    public event EventHandler OnSelectedUnitChanged;
    public Action OnSelectedUnitActionChanged;
    public EventHandler OnTakeAction;

    static public UnitActionManager Instance()
    {
        return instance;
    }
    private void Awake()
    {
        SetSelectedUnit(selectedUnit);
        if(instance)
        {
            Debug.LogError("instance��ֹһ��");
            return;
        }
        instance = this;
    }


    private void Update()
    {
        if (isBusy)
            return;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (InputManager.Instance().GetMouseButtonDown(0))
        {
            if (HandleUnitSelection())
            {
                return;
            }
            
            HandleSelectedUnitAction();
        }
    }
    private void HandleSelectedUnitAction()
    {
        if (!selectedUniAction.IsGriddPositionvalid(LevelGrid.Instance().GetGridPosition(MousePositionManager.GetMousePosition())))
        {
            return;   
        }
        if(!selectedUnit.TrySpendActionPoint(selectedUniAction.GetActionPointCost()))
        {
            return;
        }
        SetIsBusy();
        selectedUniAction.TakeAcion(LevelGrid.Instance().GetGridPosition(MousePositionManager.GetMousePosition()), ClearIsBusy);
        OnTakeAction.Invoke(this,EventArgs.Empty);
    }


    public bool HandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance().GetMousePosition());
        if(Physics.Raycast(ray, out RaycastHit rayCastHit, float.MaxValue, mask))
        {
            if (!rayCastHit.transform.gameObject.GetComponent<Unit>().GetIsEnemy())
            {
                SetSelectedUnit(rayCastHit.transform.gameObject.GetComponent<Unit>());
                return true;
            }
            else 
                return false;
        }
        else
        {
            return false;
        }
          
    }

    private void SetIsBusy()
    {
        isBusy = true;
        OnIsBusyChanged.Invoke(this, true);
    }
    private void ClearIsBusy()
    {
        isBusy = false;
        OnIsBusyChanged.Invoke(this, false);
    }

    public void SetSelectedUniAction(BaseUnitAction baseUnitAction)
    {
        this.selectedUniAction = baseUnitAction;
        OnSelectedUnitActionChanged?.Invoke();
    }
    public BaseUnitAction GetSelectedUnitAction()
    {
        return selectedUniAction;
    }


    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

        SetSelectedUniAction(selectedUnit.GetComponent<UnitMove>());
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public int GetActionPoint()
    {
        return selectedUnit.GetActionPoint();
    }

}
