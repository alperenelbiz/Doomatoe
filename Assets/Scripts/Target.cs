using UnityEngine;

public class Target : MonoBehaviour
{

    public float health = 50f;
    
    public void TakeDamage(float amount)
    {
        var damage = amount * BoostManager.instance.currentDamageBoost;
        health -= amount * BoostManager.instance.currentDamageBoost;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        EventSystem.instance.ActiveEnemies.Remove(gameObject);
        if (BoostManager.instance.dropRate >= Random.Range(0, 100))
        {
            Vector3 enemyPos = gameObject.transform.position;
            Instantiate(BoostManager.instance.boostOrb, new Vector3(enemyPos.x, enemyPos.y + .5f, enemyPos.z), Quaternion.identity);
        }
        Destroy(gameObject);
    }


}
