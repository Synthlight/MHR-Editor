local appdomain_t = sdk.find_type_definition("System.AppDomain")
local current_appdomain = appdomain_t:get_method("get_CurrentDomain")(nil)

-- Walk all assemblies
local assemblies = current_appdomain:GetAssemblies()

local system_enum_t = sdk.find_type_definition("System.Enum")
local system_enum_runtime_type = system_enum_t:get_runtime_type()

local flags_attribute_t = sdk.find_type_definition("System.FlagsAttribute")
local flags_attribute_runtime_type = flags_attribute_t:get_runtime_type()

print("Searching for enums...")

for _, assem in pairs(assemblies) do
    print("Assembly: " .. tostring(assem:get_Location()))

    local types = assem:GetTypes()

    for __, t in pairs(types) do
        local is_enum = t:IsSubclassOf(system_enum_runtime_type)

        if is_enum then
            local custom_attributes = t:call("GetCustomAttributes(System.Type, System.Boolean)", flags_attribute_runtime_type, true)

            if custom_attributes and custom_attributes:get_size() > 0 then
                print("  Flags attribute found in enum: " .. t:get_FullName())
            end
        end
    end
end