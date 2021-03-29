import matplotlib.pyplot as plt

averageFile = open('averageData.txt', 'r')
maxFile = open('maxData.txt', 'r')
branchFile = open('branch.txt', 'r')
averageData = list()
maxData = list()
for line in branchFile:
    for i in range(int(line)):
        averageData.append(list())
        maxData.append(list())
for line in averageFile:
    average = line.split()
    for enum, currAv in enumerate(average):
        averageData[enum].append(float(currAv.replace(',', '.')))
for line in maxFile:
    max = line.split()
    for enum, currMax in enumerate(max):
        maxData[enum].append(float(currMax.replace(',', '.')))
fig, ax = plt.subplots(2, 1, figsize=(len(averageData[0]), len(averageData)))

label = list()
for i in range(len(averageData)):
    ax[0].plot(range(0, len(averageData[0])), averageData[i], label="Ветвь " + str(i+1))
ax[0].set_title('Среднее значение среди отобранных')
ax[0].legend(loc='upper left')
ax[0].set_ylabel('Пройденное растояние')
ax[0].set_xlim(xmin=0, xmax=len(averageData[0])-1)
label = list()
for i in range(len(maxData)):
    ax[1].plot(range(0, len(maxData[0])), maxData[i], label="Ветвь " + str(i+1))
ax[1].set_title('Максимальное значение среди отобранных')
ax[1].legend(loc='upper left')
ax[1].set_ylabel('Пройденное растояние')
ax[1].set_xlim(xmin=0, xmax=len(maxData[0])-1)
plt.show()

