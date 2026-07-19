using UnityEngine;

namespace _Scripts.Player
{
    [System.Serializable]
    public sealed class PlayerEnergyModule
    {
        private readonly float _maxEnergy;
        private readonly float _energyRegenRate;

        private float _energy;

        public float Energy => _energy;
        public float MaxEnergy => _maxEnergy;
        
        public float BaseRegenRate => _energyRegenRate;

        public PlayerEnergyModule(float maxEnergy, float energyRegenRate, float startingEnergy)
        {
            _maxEnergy = maxEnergy;
            _energyRegenRate = energyRegenRate;
            _energy = Mathf.Clamp(startingEnergy, 0f, _maxEnergy);
        }

        public void SetEnergy(float value)
        {
            _energy = Mathf.Clamp(value, 0f, _maxEnergy);
        }

        public void ResetEnergy()
        {
            _energy = _maxEnergy;
        }

        public bool SpendEnergy(float amount)
        {
            if (amount <= 0f)
                return true;

            if (_energy < amount)
                return false;

            _energy -= amount;
            return true;
        }

        public void RegenEnergy(float dt, float bonusRate = 0f)
        {
            if (dt <= 0f || _energy >= _maxEnergy)
                return;

            float totalRegenRate =
                Mathf.Max(0f, _energyRegenRate + bonusRate);

            _energy = Mathf.Min(
                _maxEnergy,
                _energy + totalRegenRate * dt);
        }
        
        public void ApplyEnergyDelta(float amount)
        {
            if (Mathf.Approximately(amount, 0f))
                return;

            _energy = Mathf.Clamp(_energy + amount, 0f, _maxEnergy);
        }

        public void ConsumeForced(float amount)
        {
            if (amount <= 0f)
                return;

            _energy = Mathf.Max(0f, _energy - amount);
        }

        public byte QuantizeEnergy()
        {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(_energy / _maxEnergy * 255f), 0, 255);
        }

        public float DequantizeEnergy(byte b)
        {
            return b / 255f * _maxEnergy;
        }

        public void ApplyQuantizedEnergy(byte b)
        {
            _energy = DequantizeEnergy(b);
        }
    }
}