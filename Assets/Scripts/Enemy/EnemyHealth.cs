public class EnemyHealth : EntityHealth
{
    private EnemyController _enemyController;

    protected override void Start()
    {
        _enemyController = GetComponent<EnemyController>();
        base.Start();
    }

    protected override void Death()
    {
        base.Death();
        EnemyManager.Instance.RemoveController(_enemyController);
    }
}
