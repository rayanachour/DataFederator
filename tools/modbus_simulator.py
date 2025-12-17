#!/usr/bin/env python3
"""
Modbus TCP Simulator for testing DataFederator.
Simulates a Modbus slave device with holding registers.

Requirements: pip3 install pymodbus

Usage: python3 modbus_simulator.py
"""

from pymodbus.server import StartTcpServer
from pymodbus.datastore import ModbusSequentialDataBlock, ModbusSlaveContext, ModbusServerContext
import threading
import time
import random

# Holding registers with initial values (addresses 0-10)
initial_values = [100, 200, 300, 400, 500, 0, 0, 0, 0, 0, 250]

store = ModbusSlaveContext(
    hr=ModbusSequentialDataBlock(0, initial_values)
)
context = ModbusServerContext(slaves=store, single=True)

# Simulate changing values every 2 seconds
def update_values():
    while True:
        time.sleep(2)
        values = [random.randint(0, 1000) for _ in range(5)] + [0]*5 + [random.randint(200, 300)]
        store.setValues(3, 0, values)
        print(f"[SIM] Reg0={values[0]}, Reg1={values[1]}, Reg2={values[2]}, Temp={values[10]}")

threading.Thread(target=update_values, daemon=True).start()

if __name__ == "__main__":
    print("=" * 50)
    print("  MODBUS TCP SIMULATOR")
    print("  Host: 127.0.0.1  Port: 5020  Unit: 1")
    print("  Press Ctrl+C to stop")
    print("=" * 50)
    StartTcpServer(context=context, address=("127.0.0.1", 5020))

