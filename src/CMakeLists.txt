set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(SOURCE_FILES
        main.cpp
        AudioBlockObjects.h
        Interface.h
        AdmReader.h
        AudioBlockBinaural.h
        AudioBlockDirectSpeakers.h
        AudioBlockHoa.h
        AudioBlockObjects.cpp
        Interface.cpp
        AdmReader.cpp
        AudioBlockBinaural.cpp
        AudioBlockDirectSpeakers.cpp
        AudioBlockHoa.cpp
        catch.hpp
        unit_test.hpp
        unit_test.cpp
    )

if(WIN32)
    # add_executable(bw64_and_adm
        # WIN32
        # ${SOURCE_FILES}
    # )
	
	# set_target_properties(bw64_and_adm PROPERTIES
		# LINK_FLAGS /SUBSYSTEM:CONSOLE
	# )
	
elseif(APPLE)
    add_library(bw64_and_adm MODULE 
        ${SOURCE_FILES}
    )
	set_target_properties(bw64_and_adm PROPERTIES
		MACOSX_BUNDLE TRUE
		BUNDLE TRUE
	)
endif()


target_include_directories(bw64_and_adm
    PUBLIC
        #$<TARGET_PROPERTY:IRT::bw64,INCLUDE_DIRECTORIES>
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/include>
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/../submodules/libbw64/include>
        $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}>
        $<TARGET_PROPERTY:adm,INCLUDE_DIRECTORIES>
)

target_link_libraries(bw64_and_adm
    PUBLIC
      IRT::bw64
      adm
)

target_compile_features(bw64_and_adm
    PRIVATE
        cxx_std_17)

install(TARGETS bw64_and_adm
    BUNDLE DESTINATION ${CMAKE_INSTALL_PREFIX}/Unity/Assets/plugins
    LIBRARY DESTINATION ${CMAKE_INSTALL_PREFIX}/Unity/Assets/plugins
)
