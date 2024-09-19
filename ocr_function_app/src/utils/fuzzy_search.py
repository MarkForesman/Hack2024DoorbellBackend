from fuzzywuzzy import fuzz, process

# Fuzzy matching threshold
threshold = 80

def extract_name_from_label(shipping_label: str, employee_list: list[str], threshold=80):
    """Performs a fuzzy search comparison on a shipping label and a list of names.

    Args:
        shipping_label (str): The shipping label contents.
        employee_list (list[str]): List of "source of truth" employee names to compare against.
        threshold (int, optional): The fuzzy search threshold, the higher, the more strict of a
        match has to be made before returning a name. Defaults to 80.

    Returns:
        str: Name of employee from employee list of a successful fuzzy search or None if
        the threshold was not reached.
    """
    # Extract words and phrases likely to be names (heuristics: based on presence of upper-case letters)
    words = [word for word in shipping_label.split() if any(char.isupper() for char in word)]
    
    # Join consecutive potential name parts
    possible_name = ' '.join(words)

    # Perform fuzzy matching
    match = process.extractOne(possible_name, employee_list, scorer=fuzz.partial_ratio)

    # Check if the match is above the threshold
    if match[1] >= threshold:
        return match[0]
