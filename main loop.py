import time
from PIL import Image, ImageGrab
import pygetwindow as gw
import pyautogui

def png_to_2d_array(file_path):
    img = Image.open(file_path)
    width, height = img.size
    pixel_data = list(img.getdata())
    rgba_array = [pixel_data[i:i + width] for i in range(0, len(pixel_data), width)]
    return rgba_array

def create_reformatted_cum(output_folder, width, height, rgba_values, index):
    
    try:
        import os
        os.makedirs(output_folder, exist_ok=True)
        output_file_path = os.path.join(output_folder, "reformatted" + str(index) + ".chum")

        with open(output_file_path, 'wb') as file:
            file.write(width.to_bytes(2,'big'))
            file.write(height.to_bytes(2,'big'))

            for i in range(len(rgba_values)):
                for j in range(len(rgba_values[i])):
                    file.write(rgba_values[i][j][0].to_bytes(1,'big'))
                    file.write(rgba_values[i][j][1].to_bytes(1,'big'))
                    file.write(rgba_values[i][j][2].to_bytes(1,'big'))
            

        print(f"Reformatted file '{output_file_path}' created successfully.")

    except Exception as e:
        print(f"Error creating reformatted file: {e}")

    
def list_window_titles():
    # Get all open windows
    windows = gw.getAllTitles()

    if not windows:
        print("No open windows found.")
        return

    print("Open Windows:")
    for window_title in windows:
        print(f"- {window_title}")

def take_screenshot(window_title, output_file):
    target_window = gw.getWindowsWithTitle(window_title)

    if not target_window:
        print(f"Window with title '{window_title}' not found.")
        return

    # Get the coordinates of the window
    window_coords = (target_window[0].left, target_window[0].top, target_window[0].right, target_window[0].bottom)

    # Capture the screenshot of the specified window
    screenshot = ImageGrab.grab(bbox=window_coords)

    # Save the screenshot to the specified output file
    screenshot.save(output_file)
    print(f"Screenshot saved to '{output_file}'.")
    
def resize_image(input_file, output_file, target_size=(160, 90)):
    try:
        # Open the input image
        original_image = Image.open(input_file)

        # Resize the image
        resized_image = original_image.resize(target_size)

        # Save the resized image
        resized_image.save(output_file)
        print(f"Image resized and saved to '{output_file}'.")
    except Exception as e:
        print(f"Error: {e}")


#Window to capture, can see titles with the list_window_titles def
window_title_to_capture = ""  
#where screenshots are saved
screenshot_file_path = r"" 
#Where chum files are saved
chum_folder = r""
num_screenshot = 0
while(True):
    take_screenshot(window_title_to_capture, screenshot_file_path)
    resize_image(screenshot_file_path, screenshot_file_path)
    rgba_array = png_to_2d_array(screenshot_file_path)
    create_reformatted_cum(chum_folder, len(rgba_array[0]), len(rgba_array), rgba_array, num_screenshot)
    num_screenshot += 1
    time.sleep(0.1)
    