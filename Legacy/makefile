# Указываем компилятор и флаги
NASM = nasm
NASM_FLAGS = -f elf64

# Линковщик и флаги
LD = ld
LD_FLAGS = -n -o

# Папки
SRC_DIR = source
OBJ_DIR = obj
ISO_DIR = iso
BIN_FILE = $(ISO_DIR)/boot/kernel.bin
ISO_FILE = myos.iso

# Автоматически находим все .asm файлы в source
ASM_FILES = $(wildcard $(SRC_DIR)/*.asm)
OBJ_FILES = $(ASM_FILES:$(SRC_DIR)/%.asm=$(OBJ_DIR)/%.o)

# Цель по умолчанию
all: $(ISO_FILE)

# Генерация ISO-файла
$(ISO_FILE): $(BIN_FILE)
	@echo "Создаем ISO-образ..."
	grub-mkrescue -o $@ $(ISO_DIR)

# Линковка в бинарник
$(BIN_FILE): $(OBJ_FILES)
	@echo "Линкуем объектные файлы..."
	$(LD) $(LD_FLAGS) $@ $^

# Компиляция каждого .asm в .o
$(OBJ_DIR)/%.o: $(SRC_DIR)/%.asm
	@mkdir -p $(OBJ_DIR)
	@echo "Компилируем $<..."
	$(NASM) $(NASM_FLAGS) -o $@ $<

# Очистка всех сгенерированных файлов
clean:
	@echo "Удаляем скомпилированные файлы..."
	rm -rf $(OBJ_DIR) $(BIN_FILE) $(ISO_FILE)

.PHONY: all clean
