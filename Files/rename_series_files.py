from pathlib import Path
import re
import json

"""
This script will rename all files (incl. subfolders) of a given directory.
It will fix series file namings so  they will be sortable again.

before renaming: ManneZimmer - Waltreis mit Hindernis Staffel 1 Folge 1.mp4
after renaming : ManneZimmer - S01E01 - Waltreis mit Hindernis Staffel 1 Folge 1.mp4

After starting, you will be asked for every file if the rename should happen or not.
Because, if the rename does something wrong, there is no going back.
Or use 'all' to process all files without asking.

Use the `dir_path` to set your path.
Set the keywords to get the series and episode numbers right.
run command: python rename_series_files.py
"""

dir_path = Path("F:/1Serien/Mannezimmer")
series_key = 'Staffel'
# episode_key = 'Episode'
episode_key = 'Folge'


class EpisodeName:
    def __init__(self, path : Path):
        self.folder_path = path.parent.__str__()
        self.actual_file_name = path.name
        match = re.search(rf'(.*?)(?: - .*)? - (.*?) ({series_key} (\d+) {episode_key} (\d+)).*(\..*)', self.actual_file_name)
        self.series_name = match.group(1)
        self.episode_name = match.group(2)
        self.series_episode = match.group(3)
        self.series_number =  int(match.group(4))
        self.episode_number = int(match.group(5))
        self.file_type = match.group(6)
        self.new_file_name = f"{self.series_name} - S{self.series_number:02d}E{self.episode_number:02d} - {self.episode_name} {self.series_episode}{self.file_type}"
        self.new_file_name_path = f"{self.folder_path}\\{self.new_file_name}"
        self.is_name_identical = self.actual_file_name == self.new_file_name

def get_files(dir : Path):
    for path in dir.rglob('*'):
        if path.is_file():
            yield path

def is_keywords_present(dir : Path):
    return series_key in dir.name and episode_key in dir.name

files = get_files(dir_path)

print('Introduction:')
print("'abort' or 'cancel' or 'exit' to cancel the whole process")
print("'depug' to see the values of the current item")
print("any or 'skip' to skip this file")
print("'' to process this file")
print("'all' to process all files without asking")
print("Let's start!\n")

user_input = ''
file_count = 0

for f in files:
    file_count += 1
    if not (series_key in f.name and episode_key in f.name):
        print(f"ERROR: The keywords ('{series_key}', '{episode_key}') must be present in order to work but were not (file name: {f.name})")
        print('-> skipped')
        continue
    episode = EpisodeName(f)
    if episode.is_name_identical:
        print(f"skip: no changes required '{episode.actual_file_name}'")
        continue
    print(f'actual: {episode.actual_file_name}')
    print(f'rename: {episode.new_file_name}')
    print('Process? ', end='')
    if not 'all' in user_input:
        user_input = input()
    else:
        print('all')
        
    if 'debug' in user_input:
        json_str = json.dumps(episode.__dict__, indent=1)
        print(json_str)
        print('Process?')
        user_input = input()

    if 'abort' in user_input or 'cancel' in user_input or 'exit' in user_input:
        print('-> aborted')
        break
    elif bool(user_input) and not 'all' in user_input or 'skip' in user_input:
        print('-> skipped')
        continue
    else:
        f.rename(episode.new_file_name_path)
        print('-> processed')

print(f'You visited all [{file_count}] files :)')
