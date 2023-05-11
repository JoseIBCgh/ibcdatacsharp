import pandas as pd
import numpy as np
import matplotlib.pyplot as plt


def vel1(df):
    def func(row):
        prev_row = df.loc[row.name - 1]
        next_row = df.loc[row.name + 1]
        return (next_row['ang'] - prev_row['ang']) / (next_row['time'] - prev_row['time'])
    result = df.iloc[1:-1].apply(func, axis=1).to_frame('vel')
    result.index = df.index[1:-1]
    return pd.concat([df, result], axis=1)


def acc1(df):
    def func(row):
        prev_row = df.loc[row.name - 1]
        next_row = df.loc[row.name + 1]
        return (next_row['ang'] - 2 * row['ang'] + prev_row['ang']) / ((next_row['time'] - prev_row['time']) ** 2)
    result = df.iloc[1:-1].apply(func, axis=1).to_frame('acc')
    result.index = df.index[1:-1]
    return pd.concat([df, result], axis=1)


def vel2(df):
    def func_x(row):
        return row['ang_x'] + np.sin(row['ang_y']) * row['ang_z']

    def func_y(row):
        return np.cos(row['ang_x']) * row['ang_y'] - np.sin(row['ang_x']) * np.cos(row['ang_y']) * row['ang_z']

    def func_z(row):
        return np.sin(row['ang_x']) * row['ang_y'] + np.cos(row['ang_x']) * np.cos(row['ang_y']) * row['ang_z']

    vel_x = df.apply(func_x, axis=1).to_frame('vel_x')
    vel_y = df.apply(func_y, axis=1).to_frame('vel_y')
    vel_z = df.apply(func_y, axis=1).to_frame('vel_z')
    return pd.concat([df['time'], vel_x, vel_y, vel_z], axis=1)


def acc2(df):
    def func(row):
        prev_row = df.loc[row.name - 1]
        return (row['vel'] - prev_row['vel']) / (row['time'] - prev_row['time'])
    result = df.iloc[1:].apply(func, axis=1).to_frame('acc')
    result.index = df.index[1:]
    return pd.concat([df, result], axis=1)


def plots(file):
    df = pd.read_csv(file, skiprows=5, header=None, sep="\s+",
                     names=["time", "frame", "ang_x", "ang_y", "ang_z",
                            "vel_x", "vel_y", "vel_z",
                            "acc_x", "acc_y", "acc_z"]).reset_index(drop=True).astype(float)

    ang_x = df.loc[:, ['time', 'ang_x']].rename(columns={'time': 'time', 'ang_x': 'ang'})
    ang_x = vel1(ang_x)
    ang_x = acc1(ang_x)

    ang_y = df.loc[:, ['time', 'ang_y']].rename(columns={'time': 'time', 'ang_y': 'ang'})
    ang_y = vel1(ang_y)
    ang_y = acc1(ang_y)

    ang_z = df.loc[:, ['time', 'ang_z']].rename(columns={'time': 'time', 'ang_z': 'ang'})
    ang_z = vel1(ang_z)
    ang_z = acc1(ang_z)

    df2 = vel2(df)
    df2_x = df.loc[:, ['time', 'vel_x']].rename(columns={'time': 'time', 'vel_x': 'vel'})
    df2_x = acc2(df2_x)
    df2_y = df.loc[:, ['time', 'vel_y']].rename(columns={'time': 'time', 'vel_y': 'vel'})
    df2_y = acc2(df2_y)
    df2_z = df.loc[:, ['time', 'vel_z']].rename(columns={'time': 'time', 'vel_z': 'vel'})
    df2_z = acc2(df2_z)

    def plot_velx():
        fig, ax = plt.subplots()
        ang_x.plot(x='time', y='vel', ax=ax, label='method 1')
        df2_x.plot(x='time', y='vel', ax=ax, label='method 2')
        df.plot(x='time', y='vel_x', ax=ax, label='dataset')

        plt.xlabel('Time')
        plt.ylabel('Velocity')

        plt.legend()

        plt.show()

    def plot_vely():
        fig, ax = plt.subplots()
        ang_y.plot(x='time', y='vel', ax=ax, label='method 1')
        df2_y.plot(x='time', y='vel', ax=ax, label='method 2')
        df.plot(x='time', y='vel_y', ax=ax, label='dataset')

        plt.xlabel('Time')
        plt.ylabel('Velocity')

        plt.legend()

        plt.show()

    def plot_accx():
        fig, ax = plt.subplots()
        ang_x.plot(x='time', y='acc', ax=ax, label='method 1')
        df2_x.plot(x='time', y='acc', ax=ax, label='method 2')
        df.plot(x='time', y='acc_x', ax=ax, label='dataset')

        plt.xlabel('Time')
        plt.ylabel('Acceleration')

        plt.legend()

        plt.show()

    plot_velx()

file1 = "20230421-11-17-41-897.txt"
file2 = "20230421-11-29-54-999.txt"
plots(file2)
