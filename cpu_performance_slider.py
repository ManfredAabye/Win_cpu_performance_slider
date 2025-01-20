import subprocess
import tkinter as tk
from tkinter import ttk

def set_cpu_performance(min_percentage, max_percentage):
    try:
        # Setze die Prozessorleistung für den Netzbetrieb (AC) und den Akkubetrieb (DC)
        min_command_ac = f'powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMIN {min_percentage}'
        min_command_dc = f'powercfg -setdcvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMIN {min_percentage}'
        max_command_ac = f'powercfg -setacvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX {max_percentage}'
        max_command_dc = f'powercfg -setdcvalueindex SCHEME_CURRENT SUB_PROCESSOR PROCTHROTTLEMAX {max_percentage}'

        subprocess.run(min_command_ac, shell=True, check=True)
        subprocess.run(min_command_dc, shell=True, check=True)
        subprocess.run(max_command_ac, shell=True, check=True)
        subprocess.run(max_command_dc, shell=True, check=True)

        # Aktiviere den aktuellen Energiesparplan erneut, um die Änderungen sofort wirksam zu machen
        subprocess.run('powercfg -setactive SCHEME_CURRENT', shell=True, check=True)

        print(f"Mindestprozessorleistung auf {min_percentage}% und Maximalprozessorleistung auf {max_percentage}% gesetzt.")
        return True
    except subprocess.CalledProcessError as e:
        print(f"Fehler beim Setzen der Prozessorleistung: {e}")
        return False

def on_min_slider_change(event):
    min_percentage = int(min_slider.get())
    min_label.config(text=f"Mindestprozessorleistung: {min_percentage}%")

def on_max_slider_change(event):
    max_percentage = int(max_slider.get())
    max_label.config(text=f"Maximalprozessorleistung: {max_percentage}%")

def apply_settings():
    min_percentage = int(min_slider.get())
    max_percentage = int(max_slider.get())
    if set_cpu_performance(min_percentage, max_percentage):
        status_label.config(text=f"Einstellungen angewendet: Mindestleistung {min_percentage}%, Maximalleistung {max_percentage}%")
    else:
        status_label.config(text="Fehler beim Anwenden der Einstellungen")

# GUI-Erstellung mit Tkinter
root = tk.Tk()
root.title("CPU Leistungseinstellungen")

# Ändern des Fenster-Icons
root.iconbitmap('cpu_performance_slider.ico')

min_slider = ttk.Scale(root, from_=5, to=100, orient="horizontal", command=on_min_slider_change)
min_slider.pack(fill="x", padx=10, pady=10)
min_label = ttk.Label(root, text="Mindestprozessorleistung: 5%")
min_label.pack(padx=10, pady=10)

max_slider = ttk.Scale(root, from_=5, to=100, orient="horizontal", command=on_max_slider_change)
max_slider.pack(fill="x", padx=10, pady=10)
max_label = ttk.Label(root, text="Maximalprozessorleistung: 100%")
max_label.pack(padx=10, pady=10)

apply_button = ttk.Button(root, text="Einstellungen anwenden", command=apply_settings)
apply_button.pack(padx=10, pady=10)

status_label = ttk.Label(root, text="")
status_label.pack(padx=10, pady=10)

root.mainloop()
